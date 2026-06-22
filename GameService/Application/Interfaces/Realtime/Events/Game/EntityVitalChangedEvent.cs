using Contract.Enum.MetaDomain.Effect;

namespace Application.Interfaces.Realtime.Events.Game
{
    public class EntityVitalChangedEvent : IEvent
    {
        #region Attributes
        #endregion

        #region Properties
        public string EntityInstanceID { get; }
        public string RoomSpatialID { get; }
        public AttributeType AttributeType { get; }
        public float NewValue { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public EntityVitalChangedEvent(
            string entityInstanceId,
            string roomSpatialId, 
            AttributeType attributeType, 
            float newValue)
        {
            EntityInstanceID = entityInstanceId;
            RoomSpatialID = roomSpatialId;
            AttributeType = attributeType;
            NewValue = newValue;
            OccurredAt = DateTime.UtcNow;
        }

        #region Methods
        #endregion
    }
}