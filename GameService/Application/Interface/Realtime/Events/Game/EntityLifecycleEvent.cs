using Application.Interface.Realtime.Events;
using Contract.Common;
using Domain.Runtime.EntityDomain;

namespace Application.Interface.Realtime.Events.Game
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
        public Vector2? Direction { get; }
        public string RoomSpatialID { get; }
        public EntityLifecycleType Type { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public EntityLifecycleEvent(
            EntityInstance entity,
            Vector2? direction,
            string roomSpatialId,
            EntityLifecycleType type)
        {
            Entity = entity;
            Direction = direction;
            RoomSpatialID = roomSpatialId;
            Type = type;
            OccurredAt = DateTime.UtcNow;
        }

        #region Methods
        #endregion
    }
}