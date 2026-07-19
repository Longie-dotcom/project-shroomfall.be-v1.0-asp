using Application.Interfaces.Realtime;
using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Admin;
using Contract.DTO.Feature.Admin.Response;

namespace Infrastructure.Realtime.Events.Admin
{
    public class RoomStateChangedHandler : IEventHandler
    {
        #region Attributes
        private readonly IRealtimePublisher publisher;
        #endregion

        #region Properties
        #endregion

        public RoomStateChangedHandler(
            IRealtimePublisher publisher)
        {
            this.publisher = publisher;
        }

        #region Methods
        public async Task Handle(
            IEvent @event)
        {
            if (@event is not RoomStateChangedEvent e)
                return;

            await publisher.SendRoomStateChanged(
                new RoomStateChangedDTO()
                {
                    RoomSpatialID = e.RoomSpatialID,
                    PreviousState = e.OldState,
                    NewState = e.NewState
                });
        }
        #endregion
    }
}