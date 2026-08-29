using Application.Interface.Realtime.Events;

namespace Application.Interface.Realtime.Events.Admin
{
    public class RoomStateChangedEvent : IEvent
    {
        public string RoomSpatialID { get; }
        public string OldState { get; }
        public string NewState { get; }
        public DateTime OccurredAt { get; }

        public RoomStateChangedEvent(
            string roomSpatialId,
            string oldState,
            string newState)
        {
            RoomSpatialID = roomSpatialId;
            OldState = oldState;
            NewState = newState;
            OccurredAt = DateTime.UtcNow;
        }
    }
}