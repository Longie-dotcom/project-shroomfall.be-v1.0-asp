namespace Application.Interfaces.Realtime.Events.Admin
{
    public class RoomSyncChangedEvent : IEvent
    {
        public string RoomSpatialID { get; }
        public bool IsLoaded { get; }
        public DateTime OccurredAt { get; }

        public RoomSyncChangedEvent(
            string roomSpatialId,
            bool isLoaded)
        {
            RoomSpatialID = roomSpatialId;
            IsLoaded = isLoaded;
            OccurredAt = DateTime.UtcNow;
        }
    }
}