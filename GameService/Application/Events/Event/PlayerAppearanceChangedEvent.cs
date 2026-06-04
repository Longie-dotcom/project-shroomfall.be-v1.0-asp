using Application.Events.Abstraction;
using Contract.DTO.Runtime;

namespace Application.Events.Event
{
    public class PlayerAppearanceChangedEvent : IEvent
    {
        #region Attributes
        #endregion

        #region Properties
        public string EntityInstanceID { get; }
        public string RoomSpatialID { get; }
        public PlayerAppearanceRuntimeDTO Appearance { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public PlayerAppearanceChangedEvent(
            string entityInstanceId,
            string roomSpatialId,
            PlayerAppearanceRuntimeDTO appearance)
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