using Application.Events.Abstraction;

namespace Application.Events.Event
{
    public enum EntityLifecycleType
    {
        Spawn,
        Despawn
    }

    public class EntityLifecycleEvent : IEvent
    {
        #region Attributes
        #endregion

        #region Properties
        public string EntityID { get; }
        public string RoomID { get; }
        public EntityLifecycleType Type { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public EntityLifecycleEvent(
            string entityId,
            string roomId,
            EntityLifecycleType type)
        {
            EntityID = entityId;
            RoomID = roomId;
            Type = type;
            OccurredAt = DateTime.UtcNow;
        }

        #region Methods
        #endregion
    }
}