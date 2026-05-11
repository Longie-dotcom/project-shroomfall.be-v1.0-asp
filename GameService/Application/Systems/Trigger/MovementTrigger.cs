using Application.Events.Abstraction;
using Application.Events.Event;
using Application.Systems.Resolver;
using Domain.Abstraction.World;
using Domain.Runtime.EntityDomain;

namespace Application.Systems.Trigger
{
    public class MovementTrigger
    {
        #region Attributes
        private readonly IEntityCommand entityCommand;
        private readonly IWorldQuery worldQuery;
        private readonly IEventBus eventBus;
        #endregion

        #region Properties
        #endregion

        public MovementTrigger(
            IEntityCommand entityCommand,
            IWorldQuery worldQuery,
            IEventBus eventBus)
        {
            this.entityCommand = entityCommand;
            this.worldQuery = worldQuery;
            this.eventBus = eventBus;
        }

        #region Methods
        public void Apply(Dictionary<string, CollisionResult> results)
        {
            foreach (var (entityId, result) in results)
            {
                var entity = worldQuery.Get<EntityInstance>(entityId);
                if (entity == null)
                    continue;

                var oldPos = entity.Position;

                entityCommand.Move(
                    entity.ID,
                    result.FinalPosition,
                    result.LayerZ);

                if (oldPos != result.FinalPosition)
                {
                    eventBus.Publish(new EntityMovedEvent(
                        entity.ID,
                        entity.RoomSpatialID,
                        result.FinalPosition));
                }
            }
        }
        #endregion
    }
}