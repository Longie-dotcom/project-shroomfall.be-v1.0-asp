using Application.Events.Abstraction;
using Domain.Common;

namespace Application.Events.Event
{
    public class EntityMovedEvent : IEvent
    {
        #region Attributes
        #endregion

        #region Properties
        public string EntityInstanceID { get; }
        public string RoomSpatialID { get; }
        public Vector2 Position { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public EntityMovedEvent(
            string entityInstanceId,
            string roomSpatialId,
            Vector2 position)
        {
            EntityInstanceID = entityInstanceId;
            RoomSpatialID = roomSpatialId;
            Position = position;
            OccurredAt = DateTime.UtcNow;
        }

        #region Methods
        #endregion
    }
}