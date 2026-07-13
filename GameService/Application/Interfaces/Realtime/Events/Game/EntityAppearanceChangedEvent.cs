using Contract.DTO.Runtime.EntityDomain.Component;

namespace Application.Interfaces.Realtime.Events.Game
{
    public class EntityAppearanceChangedEvent : IEvent
    {
        #region Attributes
        #endregion

        #region Properties
        public string EntityInstanceID { get; }
        public string RoomSpatialID { get; }
        public AppearanceInstanceDTO Appearance { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public EntityAppearanceChangedEvent(
            string entityInstanceId,
            string roomSpatialId,
            AppearanceInstanceDTO appearance)
        {
            EntityInstanceID = entityInstanceId;
            RoomSpatialID = roomSpatialId;
            Appearance = appearance;
            OccurredAt = DateTime.UtcNow;
        }

        #region Methods
        #endregion
    }
}