using Application.Interfaces.Realtime;
using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Game;

namespace Infrastructure.Realtime.Events.Game
{
    public class RoomSnapshotUpdatedHandler : IEventHandler
    {
        #region Attributes
        private readonly IRealtimePublisher publisher;
        #endregion

        #region Properties
        #endregion

        public RoomSnapshotUpdatedHandler(
            IRealtimePublisher publisher)
        {
            this.publisher = publisher;
        }

        #region Methods
        public async Task Handle(
            IEvent @event)
        {
            if (@event is not RoomSnapshotUpdatedEvent roomEvent)
                return;

            await publisher.SendRoomSnapshotUpdated(
                roomEvent.RoomSpatialID,
                roomEvent.Room);
        }
        #endregion
    }
}