using Application.Events.Abstraction;
using Contract.DTO.Runtime;

namespace Application.Events.Event
{
    public class EntityAppearanceChangedEvent : IEvent
    {
        #region Attributes
        #endregion

        #region Properties
        public string EntityInstanceID { get; }
        public string RoomSpatialID { get; }
        public AppearanceRuntimeDTO Appearance { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public EntityAppearanceChangedEvent(
            string entityInstanceId,
            string roomSpatialId,
            AppearanceRuntimeDTO appearance)
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