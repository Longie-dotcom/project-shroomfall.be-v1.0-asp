using Application.Feature.Abstraction;
using Application.Feature.Connection.Command;
using Application.Interface.Realtime.Managers;
using Application.Service.WorldService.Run;

namespace Application.Feature.Connection.Handler
{
    public class UserDisconnectHandler : IHandler<UserDisconnectCommand>
    {
        #region Attributes
        private readonly IConnectionManager connectionManager;
        private readonly ISessionManager sessionManager;
        private readonly CombatRunService combatRunService;
        #endregion

        #region Properties
        #endregion

        public UserDisconnectHandler(
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
            UserDisconnectCommand command)
        {
            // Remove the the connection
            connectionManager.Remove(command.UserID, command.ConnectionID);

            // Handle disconnect for combat run
            var playerInstanceId = sessionManager.Get(command.UserID);
            if (playerInstanceId != null)
                combatRunService.HandlePlayerDisconnect(playerInstanceId);
        }
        #endregion
    }
}