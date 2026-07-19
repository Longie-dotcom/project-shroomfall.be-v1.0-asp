using Application.Interfaces.Realtime;
using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Admin;
using AutoMapper;
using Contract.DTO.Feature.Admin.Response;
using Contract.DTO.Runtime.WorldDomain;

namespace Infrastructure.Realtime.Events.Admin
{
    public class RoomSyncChangedHandler : IEventHandler
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly IRealtimePublisher publisher;
        #endregion

        #region Properties
        #endregion

        public RoomSyncChangedHandler(
            IMapper mapper,
            IRealtimePublisher publisher)
        {
            this.mapper = mapper;
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
                    RoomSpatial = mapper.Map<RoomSpatialDTO>(e.RoomSpatial),
                    IsLoaded = e.IsLoaded,
                });
        }
        #endregion
    }
}