using Application.Context;
using Application.Coordinator;
using Application.Events.Event;
using Application.Interfaces.Realtime;
using Application.Services.AttributeService;
using Application.Services.ItemService;
using Application.Systems.Resolver;
using Contract.Enum.AttributeDomain;
using Contract.Enum.EntityDomain;
using Domain.Runtime.EntityDomain;

namespace Application.Systems.Trigger
{
    public class AreaEffectTrigger
    {
        #region Attributes
        private readonly InventoryService inventoryService;
        private readonly WorldContext worldContext;
        private readonly EntityLifeCycleCoordinator entityLifeCycleCoordinator;
        private readonly EffectService effectService;
        private readonly IEventBus eventBus;
        #endregion

        #region Properties
        #endregion

        public AreaEffectTrigger(
            InventoryService inventoryService,
            WorldContext worldContext,
            EntityLifeCycleCoordinator entityLifeCycleCoordinator,
            EffectService effectService,
            IEventBus eventBus)
        {
            this.inventoryService = inventoryService;
            this.worldContext = worldContext;
            this.entityLifeCycleCoordinator = entityLifeCycleCoordinator;
            this.effectService = effectService;
            this.eventBus = eventBus;
        }

        #region Methods
        public void Apply(
            List<AreaEffectResult> results,
            List<string> immediateExpirations)
        {
            // 1. Clean up expired fields (e.g., fire burned out)
            foreach (var id in immediateExpirations)
            {
                var aoe = worldContext.GetEntity<AreaEffectInstance>(id);
                if (aoe != null) entityLifeCycleCoordinator.Despawn(aoe);
            }

            // 2. Process active zones
            foreach (var result in results)
            {
                var aoe = worldContext.GetEntity<AreaEffectInstance>(result.AreaEffectId);
                if (aoe == null) continue;

                // Only apply payload if the zone's internal tick-rate timer aligns
                if (!aoe.CanTickThisFrame()) continue;

                var looter = worldContext.GetEntity<CreatureInstance>(aoe.EntityInstanceOwnerID); // 🧠 Retrieve once here!

                foreach (var targetId in result.AffectedTargetIds)
                {
                    var target = worldContext.GetEntity<CreatureInstance>(targetId);
                    if (target == null) continue;

                    // Execute zone-based damage payload
                    effectService.ExecuteInstantPayload(target, aoe.SourceDefinitionID, aoe.EntityInstanceOwnerID);

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
                            // Transfer all items out of the dead entity directly to the zone owner
                            var overflows = inventoryService.TransferAllItems(target, looter);
                        }

                        // Wipe the creature entity from the simulation graph
                        entityLifeCycleCoordinator.Despawn(target);
                    }
                }
            }
        }
        #endregion
    }
}