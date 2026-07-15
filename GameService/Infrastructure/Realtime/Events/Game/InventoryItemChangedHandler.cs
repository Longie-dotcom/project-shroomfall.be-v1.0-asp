using Application.Interfaces.Realtime;
using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Game;
using Application.Interfaces.Realtime.Managers;
using AutoMapper;
using Contract.DTO.Runtime.MetaDomain;

namespace Infrastructure.Realtime.Events.Game
{
    public class InventoryItemChangedHandler : IEventHandler
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly IRealtimePublisher publisher;
        private readonly ISessionManager sessionManager;
        private readonly IConnectionManager connectionManager;
        #endregion

        public InventoryItemChangedHandler(
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
            if (@event is not InventoryItemChangedEvent changedEvent)
                return;

            var userId = sessionManager.GetUserIdByPlayerId(changedEvent.EntityInstanceID);
            if (userId == null)
                return;

            var connectionId = connectionManager.Get(userId);
            if (connectionId == null)
                return;

            await publisher.SendInventoryItemChanged(
                connectionId,
                mapper.Map<ItemInstanceDTO>(changedEvent.ItemInstance),
                changedEvent.ChangeType);
        }
        #endregion
    }
}