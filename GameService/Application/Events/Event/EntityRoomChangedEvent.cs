using Application.Events.Abstraction;

namespace Application.Events.Event
{
    public class EntityRoomChangedEvent : IEvent
    {
        #region Attributes
        #endregion

        #region Properties
        public string EntityID { get; }
        public string OldRoomID { get; }
        public string NewRoomID { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public EntityRoomChangedEvent(
            string entityId,
            string oldRoomId,
            string newRoomId)
        {
            EntityID = entityId;
            OldRoomID = oldRoomId;
            NewRoomID = newRoomId;
            OccurredAt = DateTime.UtcNow;
        }

        #region Methods
        #endregion
    }
}