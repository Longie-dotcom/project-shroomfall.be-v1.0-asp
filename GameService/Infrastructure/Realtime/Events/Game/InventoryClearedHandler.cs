using Application.Interfaces.Realtime;
using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Game;
using Application.Interfaces.Realtime.Managers;

namespace Infrastructure.Realtime.Events.Game
{
    public class InventoryClearedHandler : IEventHandler
    {
        #region Attributes
        private readonly IRealtimePublisher publisher;
        private readonly ISessionManager sessionManager;
        private readonly IConnectionManager connectionManager;
        #endregion

        public InventoryClearedHandler(
            IRealtimePublisher publisher,
            ISessionManager sessionManager,
            IConnectionManager connectionManager)
        {
            this.publisher = publisher;
            this.sessionManager = sessionManager;
            this.connectionManager = connectionManager;
        }

        #region Methods
        public async Task Handle(
            IEvent @event)
        {
            if (@event is not InventoryClearedEvent clearedEvent)
                return;

            var userId = sessionManager.GetUserIdByPlayerId(clearedEvent.EntityInstanceID);
            if (userId == null)
                return;

            var connectionId = connectionManager.Get(userId);
            if (connectionId == null)
                return;

            await publisher.SendInventoryCleared(connectionId);
        }
        #endregion
    }
}