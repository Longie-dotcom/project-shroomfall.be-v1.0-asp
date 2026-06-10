using Application.Events.Abstraction;
using Contract.Enum.EntityDomain;
using Domain.Common;

namespace Application.Events.Event
{
    public class EntityActedEvent : IEvent
    {
        #region Attributes
        #endregion

        #region Properties
        public string EntityInstanceID { get; }
        public string RoomSpatialID { get; }
        public Vector2 Position { get; }
        public EntityDirection Direction { get; }
        public EntityAction Action { get; }
        public string? UsedItemDefinitionID { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public EntityActedEvent(
            string entityInstanceId,
            string roomSpatialId,
            Vector2 position,
            EntityDirection direction,
            EntityAction action,
            string? usedItemDefinitionId)
        {
            EntityInstanceID = entityInstanceId;
            RoomSpatialID = roomSpatialId;
            Position = position;
            Direction = direction;
            Action = action;
            OccurredAt = DateTime.UtcNow;
            UsedItemDefinitionID = usedItemDefinitionId;
        }

        #region Methods
        #endregion
    }
}