using Application.Events.Abstraction;

namespace Application.Events.Event
{
    public class PlayerGroupedEvent : IEvent
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public string? OldRoomSpatialID { get; }
        public string? NewRoomSpatialID { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public PlayerGroupedEvent(
        string userId,
        string? oldRoomSpatialId,
        string? newRoomSpatialId)
        {
            UserID = userId;
            OldRoomSpatialID = oldRoomSpatialId;
            NewRoomSpatialID = newRoomSpatialId;
            OccurredAt = DateTime.Now;
        }

        #region Methods
        #endregion
    }
}