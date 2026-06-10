using Application.Events.Abstraction;
using Application.Events.Event;
using Application.Interfaces.Realtime;
using Application.Interfaces.Security;

namespace Infrastructure.Realtime.Handlers
{
    public class PlayerCharacteristicSyncHandler : IEventHandler
    {
        #region Attributes
        private readonly IRealtimePublisher publisher;
        private readonly ISessionManager sessionManager;
        private readonly IConnectionRegistry connectionRegistry;
        #endregion

        #region Properties
        #endregion

        public PlayerCharacteristicSyncHandler(
                IRealtimePublisher publisher,
                ISessionManager sessionManager,
                IConnectionRegistry connectionRegistry)
        {
            this.publisher = publisher;
            this.sessionManager = sessionManager;
            this.connectionRegistry = connectionRegistry;
        }

        #region Methods
        public async Task Handle(IEvent @event)
        {
            if (@event is not PlayerCharacteristicSyncEvent syncEvent)
                return;

            var userId = sessionManager.GetUserIdByPlayerId(syncEvent.EntityInstanceID);
            if (userId != null)
            {
                var connectionIds = connectionRegistry.Get(userId);

                // Dispatch specifically to those connection pipes
                foreach (var connId in connectionIds)
                {
                    await publisher.SendPlayerCharacteristicSync(connId, syncEvent.CharacteristicRuntime);
                }
            }
        }
        #endregion
    }
}