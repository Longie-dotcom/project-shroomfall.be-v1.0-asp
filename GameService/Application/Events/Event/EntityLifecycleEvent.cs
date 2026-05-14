using Application.Events.Abstraction;
using Domain.Runtime.EntityDomain;

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
        public EntityInstance Entity { get; }
        public string RoomSpatialID { get; }
        public EntityLifecycleType Type { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public EntityLifecycleEvent(
            EntityInstance entity,
            string roomSpatialId,
            EntityLifecycleType type)
        {
            Entity = entity;
            RoomSpatialID = roomSpatialId;
            Type = type;
            OccurredAt = DateTime.UtcNow;
        }

        #region Methods
        #endregion
    }
}