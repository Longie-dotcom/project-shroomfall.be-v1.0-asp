using Application.Events.Abstraction;
using Domain.Common;

namespace Application.Events.Event
{
    public class EntityMovedEvent : IEvent
    {
        #region Attributes
        #endregion

        #region Properties
        public string EntityID { get; }
        public string RoomID { get; }
        public Vector2 Position { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public EntityMovedEvent(
            string entityId, 
            string roomId,
            Vector2 position)
        {
            EntityID = entityId;
            RoomID = roomId;
            Position = position;
            OccurredAt = DateTime.UtcNow;
        }

        #region Methods
        #endregion
    }
}