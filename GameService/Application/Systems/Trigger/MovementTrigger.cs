using Application.Context;
using Application.Events.Event;
using Application.Interfaces.Realtime;
using Application.Systems.Resolver;
using Domain.Runtime.EntityDomain;

namespace Application.Systems.Trigger
{
    public class MovementTrigger
    {
        #region Attributes
        private readonly WorldContext worldContext;
        private readonly IEventBus eventBus;
        #endregion

        #region Properties
        #endregion

        public MovementTrigger(
            WorldContext worldContext,
            IEventBus eventBus)
        {
            this.worldContext = worldContext;
            this.eventBus = eventBus;
        }

        #region Methods
        public void Apply(
            Dictionary<string, CollisionResult> results)
        {
            foreach (var (entityId, result) in results)
            {
                // Retrieve entity instance from runtime
                var entity = worldContext.GetEntity<EntityInstance>(entityId);
                if (entity == null)
                    return;

                // Update context and spatial indexing
                worldContext.EntityMove(
                    entity.ID,
                    result.FinalPosition,
                    result.LayerZ);

                // Only publish event when position is new
                if (!entity.Position.NearlyEquals(entity.Position))
                {
                    eventBus.Publish(new EntityMovedEvent(
                        entity.ID,
                        entity.RoomSpatialID,
                        entity.Position));
                }
            }
        }
        #endregion
    }
}