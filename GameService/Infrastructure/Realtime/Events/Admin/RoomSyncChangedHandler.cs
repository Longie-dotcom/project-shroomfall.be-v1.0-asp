using Application.Interfaces.Realtime;
using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Admin;
using Contract.DTO.Feature.Admin.Response;

namespace Infrastructure.Realtime.Events.Admin
{
    public class RoomSyncChangedHandler : IEventHandler
    {
        #region Attributes
        private readonly IRealtimePublisher publisher;
        #endregion

        #region Properties
        #endregion

        public RoomSyncChangedHandler(
            IRealtimePublisher publisher)
        {
            this.publisher = publisher;
        }

        #region Methods
        public async Task Handle(
            IEvent @event)
        {
            if (@event is not RoomSyncChangedEvent e)
                return;

            await publisher.SendRoomSyncChanged(
                new RoomSyncChangedDTO()
                {
                    RoomSpatialID = e.RoomSpatialID,
                    IsLoaded = e.IsLoaded,
                });
        }
        #endregion
    }
}