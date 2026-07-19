using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Game;
using Application.Services.AttributeService;
using Application.Services.EntityService;
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
        private readonly IEventBus eventBus;
        private readonly ItemService itemService;
        private readonly ProjectileService projectileService;
        private readonly TriggeredEffectService triggeredEffectService;
        private readonly EntitySpawnService entitySpawnService;
        private readonly InventoryService inventoryService;
        #endregion

        public EntityTrigger(
            WorldContext worldContext,
            IEventBus eventBus,
            ItemService itemService,
            ProjectileService projectileService,
            TriggeredEffectService triggeredEffectService,
            EntitySpawnService entitySpawnService,
            InventoryService inventoryService)
        {
            this.worldContext = worldContext;
            this.eventBus = eventBus;
            this.itemService = itemService;
            this.projectileService = projectileService;
            this.triggeredEffectService = triggeredEffectService;
            this.entitySpawnService = entitySpawnService;
            this.inventoryService = inventoryService;
        }

        #region Methods
        public void Apply(
            CommandBuffer commandBuffer)
        {
            while (commandBuffer.Results.TryDequeue(out var result))
            {
                switch (result)
                {
                    case MovementResult movementRes:
                        ApplyMovement(movementRes, commandBuffer);
                        break;

                    case ItemActionResult itemActionRes:
                        ApplyItemAction(itemActionRes);
                        break;

                    case EntityExpiredResult entityExpiredRes:
                        ApplyEntityExpired(entityExpiredRes, commandBuffer);
                        break;

                    case VitalThresholdResult vitalThresholdRes:
                        ApplyVitalThreshold(vitalThresholdRes, commandBuffer);
                        break;

                    case EntityDespawnResult entityDespawnRes:
                        ApplyEntityDespawn(entityDespawnRes);
                        break;
                }
            }
        }

        private void ApplyMovement(
                    MovementResult result,
                    CommandBuffer commandBuffer)
        {
            // 1. Log entry check
            var entity = worldContext.GetEntity(result.EntityInstanceID);
            if (entity == null)
            {
                Console.WriteLine($"[Movement Processing] FAILED: Entity ID '{result.EntityInstanceID}' not found in world context.");
                return;
            }

            var transform = entity.GetComponent<TransformInstance>();
            if (transform == null)
            {
                Console.WriteLine($"[Movement Processing] FAILED: Entity ID '{entity.ID}' is missing its TransformInstance component.");
                return;
            }

            // Cache state before updating position index
            bool wasMoving = transform.WantsToMove || transform.PositionChangedThisFrame;

            Console.WriteLine($"[Movement Processing] Processing entity '{entity.ID}'. " +
                              $"Current Pos: {transform.Position}, " +
                              $"Target Pos: {result.FinalPosition}, " +
                              $"Layer: {result.LayerZ}, " +
                              $"WasMoving: {wasMoving}");

            // Update context and spatial indexing
            worldContext.EntityMove(
                entity.ID,
                result.FinalPosition,
                result.LayerZ);

            // Determine network sync conditions
            bool hasMoved = transform.PositionChangedThisFrame;
            bool justStopped = wasMoving && !transform.WantsToMove;

            // Send network update if they moved, OR if they just stopped moving
            if (hasMoved || justStopped)
            {
                Console.WriteLine($"[Movement Processing] Network sync triggered for '{entity.ID}'. Reason: " +
                                  $"{(hasMoved ? "PositionChangedThisFrame" : "")} {(justStopped ? "JustStoppedMoving" : "")} | " +
                                  $"Sending Pos: {transform.Position}, Dir: {transform.FacingDirection}, Action: {transform.CurrentAction}");

                eventBus.Publish(new EntityActedEvent(
                    entity.ID,
                    transform.RoomSpatialID,
                    transform.Position,
                    transform.FacingDirection,
                    transform.CurrentAction,
                    null
                ));
            }
            else
            {
                Console.WriteLine($"[Movement Processing] Movement evaluated for '{entity.ID}', but no network sync sent (Positions matching/idle).");
            }

            foreach (var touched in result.TriggeredEntities)
            {
                Console.WriteLine($"[Movement Processing] Entity '{entity.ID}' touched entity '{touched.ID}'");

                triggeredEffectService.OnEntityTouched(touched, entity);

                if (projectileService.TryHandleImpact(entity))
                {
                    Console.WriteLine($"[Movement Processing] Projectile impact detected for '{entity.ID}'. Enqueuing Despawn.");
                    commandBuffer.Commands.Enqueue(new EntityDespawnCommand(entity.ID, false));
                }

                if (inventoryService.TryPickItem(entity, touched))
                {
                    Console.WriteLine($"[Movement Processing] Item pick success. '{entity.ID}' picked up '{touched.ID}'. Enqueuing item Despawn.");
                    commandBuffer.Commands.Enqueue(new EntityDespawnCommand(touched.ID, false));
                }
            }
        }

        private void ApplyItemAction(
            ItemActionResult result)
        {
            var entity = worldContext.GetEntity(result.EntityInstanceID);
            if (entity == null) 
                return;

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

        private void ApplyEntityExpired(
            EntityExpiredResult result,
            CommandBuffer commandBuffer)
        {
            var entity = worldContext.GetEntity(result.EntityInstanceID);
            if (entity == null) 
                return;

            // Apply projectile logic
            projectileService.TryHandleImpact(entity);

            commandBuffer.Commands.Enqueue(new EntityDespawnCommand(entity.ID, false));
        }

        private void ApplyVitalThreshold(
            VitalThresholdResult result,
            CommandBuffer commandBuffer)
        {
            switch (result.Outcome)
            {
                case DeathOutcome.Entity:
                    commandBuffer.Commands.Enqueue(new EntityDespawnCommand(result.EntityInstanceID, true));
                    break;

                case DeathOutcome.Player:
                    // publish the Run mode of that participant
                    break; 

                case DeathOutcome.None:
                default:
                    break;
            }
        }

        private void ApplyEntityDespawn(
            EntityDespawnResult result)
        {
            var entity = worldContext.GetEntity(result.EntityInstanceID);
            if (entity == null)
                return;

            if (result.TriggerDeathLogic)
            {
                var transform = entity.GetComponent<TransformInstance>();

                if (transform != null)
                {
                    var drops = inventoryService.DropAllItems(entity);

                    foreach (var item in drops)
                    {
                        entitySpawnService.Spawn(
                            new WorldItemCreateContext(
                                Guid.NewGuid().ToString(),
                                transform.RoomSpatialID,
                                transform.LayerZ,
                                transform.Position,
                                item));
                    }
                }
            }

            entitySpawnService.Despawn(entity);
        }
        #endregion
    }
}