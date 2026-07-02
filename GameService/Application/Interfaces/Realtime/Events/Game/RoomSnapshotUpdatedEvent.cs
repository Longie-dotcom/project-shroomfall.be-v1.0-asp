using Contract.DTO.Connection;

namespace Application.Interfaces.Realtime.Events.Game
{
    public class RoomSnapshotUpdatedEvent
    {
        #region Attributes
        #endregion

        #region Properties
        public string RoomSpatialID { get; }
        public RoomSnapshotDTO Room { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public RoomSnapshotUpdatedEvent(
            string roomSpatialId,
            RoomSnapshotDTO room)
        {
            RoomSpatialID = roomSpatialId;
            Room = room;
            OccurredAt = DateTime.UtcNow;
        }

        #region Methods
        #endregion
    }
}