using Application.Interface.Realtime;
using Application.Interface.Realtime.Events;
using Application.Interface.Realtime.Events.Game;
using Application.Interface.Realtime.Managers;
using AutoMapper;
using Contract.DTO.Runtime.EntityDomain.Component;

namespace Infrastructure.Realtime.Events.Game
{
    public class PlayerCharacteristicSyncHandler : IEventHandler
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly IRealtimePublisher publisher;
        private readonly ISessionManager sessionManager;
        private readonly IConnectionManager connectionManager;
        #endregion

        #region Properties
        #endregion

        public PlayerCharacteristicSyncHandler(
            IMapper mapper,
            IRealtimePublisher publisher,
            ISessionManager sessionManager,
            IConnectionManager connectionManager)
        {
            this.mapper = mapper;
            this.publisher = publisher;
            this.sessionManager = sessionManager;
            this.connectionManager = connectionManager;
        }

        #region Methods
        public async Task Handle(
            IEvent @event)
        {
            if (@event is not PlayerCharacteristicSyncEvent syncEvent)
                return;

            var userId = sessionManager.GetUserIdByPlayerId(syncEvent.EntityInstanceID);
            if (userId == null)
                return;

            var connectionId = connectionManager.Get(userId);
            if (connectionId == null)
                return;

            await publisher.SendPlayerCharacteristicSync(
                connectionId,
                mapper.Map<CharacteristicInstanceDTO>(syncEvent.CharacteristicInstance));
        }
        #endregion
    }
}