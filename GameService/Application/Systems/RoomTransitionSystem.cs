using Application.Events.Abstraction;
using Application.Events.Event;
using Domain.Abstraction.World;
using Domain.Runtime.EntityDomain;

namespace Application.Systems
{
    public class RoomTransitionSystem
    {
        #region Attributes
        private readonly IWorldQuery world;
        private readonly IEventBus bus;
        private readonly Dictionary<string, string> lastRoom = new();
        #endregion

        #region Properties
        #endregion

        public RoomTransitionSystem(
            IWorldQuery world,
            IEventBus bus)
        {
            this.world = world;
            this.bus = bus;
        }

        #region Methods
        public void Update()
        {
            foreach (var entity in world.GetAll<EntityInstance>())
            {
                if (!lastRoom.TryGetValue(entity.ID, out var prevRoom))
                {
                    lastRoom[entity.ID] = entity.RoomSpatialID;
                    continue;
                }

                if (prevRoom == entity.RoomSpatialID)
                    continue;

                bus.Publish(new EntityRoomChangedEvent(
                    entity.ID,
                    prevRoom,
                    entity.RoomSpatialID));

                lastRoom[entity.ID] = entity.RoomSpatialID;
            }
        }
        #endregion
    }
}