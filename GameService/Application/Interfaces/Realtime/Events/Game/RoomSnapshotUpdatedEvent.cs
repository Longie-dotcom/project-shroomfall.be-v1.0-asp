using Contract.DTO.Runtime.WorldDomain;

namespace Application.Interfaces.Realtime.Events.Game
{
    public class RoomSnapshotUpdatedEvent
    {
        #region Attributes
        #endregion

        #region Properties
        public string RoomSpatialID { get; }
        public RoomSpatialDTO Room { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public RoomSnapshotUpdatedEvent(
            string roomSpatialId,
            RoomSpatialDTO room)
        {
            RoomSpatialID = roomSpatialId;
            Room = room;
            OccurredAt = DateTime.UtcNow;
        }

        #region Methods
        #endregion
    }
}