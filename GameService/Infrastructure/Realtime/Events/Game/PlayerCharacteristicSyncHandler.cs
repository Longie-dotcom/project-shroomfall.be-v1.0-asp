using Application.Interfaces.Realtime;
using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Game;
using Application.Interfaces.Realtime.Managers;
using AutoMapper;
using Contract.DTO.Domain.Runtime;

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
            if (userId != null)
            {
                var connectionIds = connectionManager.Get(userId);

                foreach (var connId in connectionIds)
                {
                    await publisher.SendPlayerCharacteristicSync(
                        connId,
                        mapper.Map<CharacteristicInstanceDTO>(syncEvent.CharacteristicInstance));
                }
            }
        }
        #endregion
    }
}