using Application.Context;
using Application.Coordinator;
using Application.Events.Event;
using Application.Interfaces.Realtime;
using Application.Services.AttributeService;
using Application.Services.ItemService;
using Application.Systems.Resolver;
using Contract.Enum.AttributeDomain;
using Contract.Enum.EntityDomain;
using Domain.Common;
using Domain.Runtime.EntityDomain;

namespace Application.Systems.Trigger
{
    public class ProjectileTrigger
    {
        #region Attributes
        private readonly InventoryService inventoryService;
        private readonly WorldContext worldContext;
        private readonly EntityLifeCycleCoordinator entityLifeCycleCoordinator;
        private readonly IEventBus eventBus;
        private readonly EffectService effectService;
        #endregion

        #region Properties
        #endregion

        public ProjectileTrigger(
            InventoryService inventoryService,
            WorldContext worldContext,
            EntityLifeCycleCoordinator entityLifeCycleCoordinator,
            IEventBus eventBus,
            EffectService effectService)
        {
            this.inventoryService = inventoryService;
            this.worldContext = worldContext;
            this.entityLifeCycleCoordinator = entityLifeCycleCoordinator;
            this.eventBus = eventBus;
            this.effectService = effectService;
        }

        #region Methods
        public void Apply(
            Dictionary<string, ProjectileResult> results,
            List<string> immediateExpirations)
        {
            // 1. Clean up projectiles that naturally timed out (e.g., reached throw destination)
            foreach (var id in immediateExpirations)
            {
                var proj = worldContext.GetEntity<ProjectileInstance>(id);
                if (proj != null)
                {
                    // Check the runtime list passed by your factory
                    TrySpawnAreaEffects(proj, proj.Position, EntityRelationshipType.Throwable);

                    entityLifeCycleCoordinator.Despawn(proj);
                }
            }

            // 2. Process physical impacts
            foreach (var (id, result) in results)
            {
                var proj = worldContext.GetEntity<ProjectileInstance>(id);
                if (proj == null) continue;

                if (result.DidImpact)
                {
                    // Spawns the zone right where the impact occurred
                    TrySpawnAreaEffects(proj, result.FinalPosition, EntityRelationshipType.Throwable);

                    // Evaluate impact calculations for EVERY target caught inside the intersection box
                    var looter = worldContext.GetEntity<CreatureInstance>(proj.EntityInstanceOwnerID); // 🧠 Retrive once here!

                    foreach (var targetId in result.HitTargetIds)
                    {
                        var target = worldContext.GetEntity<CreatureInstance>(targetId);
                        if (target == null) continue;

                        // Apply damage payload (make sure to pass OwnerID down to your new routing service logic)
                        effectService.ExecuteInstantPayload(target, proj.SourceDefinitionID, proj.EntityInstanceOwnerID);

                        // 💥 Publish unified DAMAGED event
                        eventBus.Publish(new EntityActedEvent(
                            entityInstanceId: target.ID,
                            roomSpatialId: target.RoomSpatialID,
                            position: target.Position,
                            direction: target.FacingDirection,
                            action: EntityAction.DAMAGED,
                            usedItemDefinitionId: null
                        ));

                        // 💀 Target Died Rule
                        if (target.Characteristic.GetVital(AttributeType.Health) <= 0f)
                        {
                            if (looter != null)
                            {
                                // Execute inventory transaction safely
                                var overflows = inventoryService.TransferAllItems(target, looter);
                            }

                            // Wipe the creature entity from the simulation graph
                            entityLifeCycleCoordinator.Despawn(target);
                        }
                    }

                    entityLifeCycleCoordinator.Despawn(proj);
                }
                else
                {
                    worldContext.EntityMove(proj.ID, result.FinalPosition, proj.LayerZ);
                }
            }
        }

        private void TrySpawnAreaEffects(
            ProjectileInstance proj,
            Vector2 spawnPosition,
            EntityRelationshipType actionType)
        {
            // Instantly check if the projectile has any configured behaviors for this specific action type
            if (proj.Relationships.TryGetValue(actionType, out var targetAoEDefinitionIds))
            {
                foreach (var aoeDefId in targetAoEDefinitionIds)
                {
                    entityLifeCycleCoordinator.SpawnAreaEffect(
                        areaEffectDefinitionId: aoeDefId,
                        roomSpatialId: proj.RoomSpatialID,
                        layerZ: proj.LayerZ,
                        position: spawnPosition,
                        ownerId: proj.EntityInstanceOwnerID,
                        sourceDefinitionId: proj.SourceDefinitionID
                    );
                }
            }
        }
        #endregion
    }
}