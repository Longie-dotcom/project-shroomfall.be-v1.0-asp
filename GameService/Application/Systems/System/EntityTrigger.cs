using Application.Interfaces.Cache;
using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Game;
using Application.Services.AttributeService;
using Application.Services.UsageService;
using Application.Services.WorldService;
using Application.Systems.Queue;
using Domain.Runtime.EntityDomain.Component;

namespace Application.Systems.System
{
    public class EntityTrigger
    {
        #region Attributes
        private readonly WorldContext worldContext;
        private readonly ICacheProvider cacheProvider;
        private readonly IEventBus eventBus;
        private readonly EntitySpawnService entitySpawnService;
        private readonly EffectService effectService;
        private readonly ItemService itemService;
        #endregion

        public EntityTrigger(
            WorldContext worldContext,
            ICacheProvider cacheProvider,
            IEventBus eventBus,
            EntitySpawnService entitySpawnService,
            EffectService effectService,
            ItemService itemService)
        {
            this.worldContext = worldContext;
            this.cacheProvider = cacheProvider;
            this.eventBus = eventBus;
            this.entitySpawnService = entitySpawnService;
            this.effectService = effectService;
            this.itemService = itemService;
        }

        #region Methods
        public void Apply(
            CommandBuffer commandBuffer)
        {
            while (commandBuffer.Results.TryDequeue(out var result))
            {
                switch (result)
                {
                    case MovementResult moveRes:
                        ApplyMovement(moveRes);
                        break;

                    case ItemActionResult itemRes:
                        ApplyItemAction(itemRes);
                        break;

                    case DespawnResult despawnRes:
                        ApplyDespawn(despawnRes);
                        break;
                }
            }
        }

        private void ApplyMovement(
            MovementResult result)
        {
            var entity = worldContext.GetEntity(result.EntityInstanceID);
            if (entity == null) return;

            var transform = entity.GetComponent<TransformInstance>();
            if (transform == null) return;

            // Cache state before updating position index
            bool wasMoving = transform.WantsToMove || transform.PositionChangedThisFrame;

            // Update context and spatial indexing
            worldContext.EntityMove(
                entity.ID,
                result.FinalPosition,
                result.LayerZ);

            // Send network update if they moved, OR if they just stopped moving
            if (transform.PositionChangedThisFrame || (wasMoving && !transform.WantsToMove))
            {
                eventBus.Publish(new EntityActedEvent(
                    entity.ID,
                    transform.RoomSpatialID,
                    transform.Position,
                    transform.FacingDirection,
                    transform.CurrentAction,
                    null
                ));
            }

            foreach (var touched in result.TriggeredEntities)
            {
                // Scenario A: The entity we stepped on has the effect payload (e.g., Player walks into a Spike Trap)
                var trapEffect = touched.GetComponent<TriggeredEffectInstance>();
                if (trapEffect != null)
                {
                    foreach (var effectId in trapEffect.EffectDefinitionIDs)
                    {
                        effectService.ApplyEffect(entity, effectId);
                    }
                }

                // Scenario B: WE are the ones carrying the effect payload (e.g., Fireball flies into a stationary Player)
                var myEffect = entity.GetComponent<TriggeredEffectInstance>();
                if (myEffect != null)
                {
                    foreach (var effectId in myEffect.EffectDefinitionIDs)
                    {
                        effectService.ApplyEffect(entity, effectId);
                    }
                }
            }
        }

        private void ApplyItemAction(
            ItemActionResult result)
        {
            var entity = worldContext.GetEntity(result.EntityInstanceID);
            if (entity == null) return;

            // Execute the item usage logic
            itemService.Execute(entity, result.Context);

            // Grab the transform to get current position/facing direction
            var transform = entity.GetComponent<TransformInstance>();
            if (transform != null && result.Context.ItemDef.TriggeredAction.HasValue)
            {
                eventBus.Publish(new EntityActedEvent(
                    entity.ID,
                    transform.RoomSpatialID,
                    transform.Position,
                    transform.FacingDirection,
                    result.Context.ItemDef.TriggeredAction.Value,
                    result.Context.ItemDef.ID
                ));
            }
        }

        private void ApplyDespawn(
            DespawnResult result)
        {
            var entity = worldContext.GetEntity(result.EntityInstanceID);
            if (entity == null) return;

            var transform = entity.GetComponent<TransformInstance>();
            var projectile = entity.GetComponent<ProjectileInstance>();

            // Handle Projectile side effects (e.g., throwing a weapon that transitions to an AOE)
            if (projectile != null && transform != null && !string.IsNullOrEmpty(projectile.OnImpactSpawnEntityDefinitionID))
            {
                var spawnContext = new WorldEntityCreateContext(
                    Guid.NewGuid().ToString(),
                    projectile.OnImpactSpawnEntityDefinitionID,
                    transform.RoomSpatialID,
                    transform.LayerZ,
                    transform.Position
                );

                // Trigger the new spawn instantly in the execution phase
                entitySpawnService.Spawn(spawnContext);
            }
        }
        #endregion
    }
}