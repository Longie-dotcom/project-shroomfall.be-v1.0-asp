using Application.Context;
using Application.Events.Event;
using Application.Interfaces.Realtime;
using Application.Systems.Resolver;
using Domain.Runtime.EntityDomain;

namespace Application.Systems.Trigger
{
    public class CreatureTrigger
    {
        #region Attributes
        private readonly WorldContext worldContext;
        private readonly IEventBus eventBus;
        #endregion

        #region Properties
        #endregion

        public CreatureTrigger(
            WorldContext worldContext,
            IEventBus eventBus)
        {
            this.worldContext = worldContext;
            this.eventBus = eventBus;
        }

        #region Methods
        public void Apply(
            Dictionary<string, CreatureResult> results)
        {
            foreach (var (entityId, result) in results)
            {
                // Retrieve entity instance from runtime
                var entity = worldContext.GetEntity<EntityInstance>(entityId);
                if (entity == null)
                    continue;

                // Cache state before updating position index
                bool wasMoving = entity.WantsToMove || entity.PositionChangedThisFrame;

                // Update context and spatial indexing
                worldContext.EntityMove(
                    entity.ID,
                    result.FinalPosition,
                    result.LayerZ);

                // Send update if they moved, OR if they just stopped moving (to settle the client)
                if (entity.PositionChangedThisFrame || (wasMoving && !entity.WantsToMove))
                {
                    eventBus.Publish(new EntityActedEvent(
                        entity.ID,
                        entity.RoomSpatialID,
                        entity.Position,
                        entity.FacingDirection,
                        entity.CurrentAction,
                        null
                    ));
                }
            }
        }
        #endregion
    }
}