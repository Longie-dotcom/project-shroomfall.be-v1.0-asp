using Application.Events.Abstraction;
using Application.Events.Event;
using Application.Interfaces.Realtime;

namespace Infrastructure.Realtime.Handlers
{
    public class PlayerGroupedHandler : IEventHandler
    {
        #region Attributes
        private readonly IConnectionRegistry connectionRegistry;
        private readonly IConnectionManager connectionManager;
        #endregion

        #region Properties
        #endregion

        public PlayerGroupedHandler(
            IConnectionRegistry connectionRegistry,
            IConnectionManager connectionManager)
        {
            this.connectionRegistry = connectionRegistry;
            this.connectionManager = connectionManager;
        }

        #region Methods
        public async Task Handle(
            IEvent @event)
        {
            if (@event is not PlayerGroupedEvent e)
                return;

            var connectionIds = connectionRegistry.Get(e.UserID);

            if (!connectionIds.Any())
                return;

            foreach (var connectionId in connectionIds)
            {
                if (!string.IsNullOrEmpty(e.OldRoomSpatialID))
                {
                    await connectionManager.LeaveAsync(connectionId, e.OldRoomSpatialID);
                }

                if (!string.IsNullOrEmpty(e.NewRoomSpatialID))
                {
                    await connectionManager.JoinAsync(connectionId, e.NewRoomSpatialID);
                }
            }
        }
        #endregion
    }
}