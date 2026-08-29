using Application.Feature.Abstraction;
using Application.Feature.Connection.Command;
using Application.Interface.Realtime.Managers;
using Application.Service.WorldService.Run;

namespace Application.Feature.Connection.Handler
{
    public class UserConnectHandler : IHandler<UserConnectCommand>
    {
        #region Attributes
        private readonly IConnectionManager connectionManager;
        private readonly ISessionManager sessionManager;
        private readonly CombatRunService combatRunService;
        #endregion

        #region Properties
        #endregion

        public UserConnectHandler(
            IConnectionManager connectionManager,
            ISessionManager sessionManager,
            CombatRunService combatRunService)
        {
            this.connectionManager = connectionManager;
            this.sessionManager = sessionManager;
            this.combatRunService = combatRunService;
        }

        #region Methods
        public async Task Handle(
            UserConnectCommand command)
        {
            // Register the the connection (one connection per user only, replaced with old connection)
            connectionManager.Add(command.UserID, command.ConnectionID);

            // Handle reconnect for combat run
            var playerInstanceId = sessionManager.Get(command.UserID);
            if (playerInstanceId != null)
                combatRunService.HandlePlayerReconnect(playerInstanceId);
        }
        #endregion
    }
}