using Application.Interface.Realtime.Events;
using Domain.Runtime.WorldDomain.Spatial;

namespace Application.Interface.Realtime.Events.Admin
{
    public class RoomSyncChangedEvent : IEvent
    {
        public RoomSpatial RoomSpatial { get; }
        public bool IsLoaded { get; }
        public DateTime OccurredAt { get; }

        public RoomSyncChangedEvent(
            RoomSpatial roomSpatial,
            bool isLoaded)
        {
            RoomSpatial = roomSpatial;
            IsLoaded = isLoaded;
            OccurredAt = DateTime.UtcNow;
        }
    }
}