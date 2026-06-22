using Application.Interfaces.Realtime;
using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Admin;
using Contract.DTO.Admin;

namespace Infrastructure.Realtime.Events.Admin
{
    public class RoomResidencyChangedHandler : IEventHandler
    {
        #region Attributes
        private readonly IRealtimePublisher publisher;
        #endregion

        #region Properties
        #endregion

        public RoomResidencyChangedHandler(
            IRealtimePublisher publisher)
        {
            this.publisher = publisher;
        }

        #region Methods
        public async Task Handle(
            IEvent @event)
        {
            if (@event is not RoomResidencyChangedEvent e)
                return;

            await publisher.SendRoomResidencyChanged(
                new RoomResidencyChangedDTO()
                {
                    RoomSpatialID = e.RoomSpatialID,
                    PreviousState = e.PreviousState,
                    NewState = e.NewState
                });
        }
        #endregion
    }
}