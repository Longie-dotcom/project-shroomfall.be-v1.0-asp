using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
using Application.Interfaces.Realtime.Managers;

namespace Application.Features.Connection.Handlers
{
    public class UserConnectHandler : IHandler<UserConnectCommand>
    {
        #region Attributes
        private readonly IConnectionManager connectionManager;
        #endregion

        #region Properties
        #endregion

        public UserConnectHandler(
            IConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager;
        }

        #region Methods
        public async Task Handle(
            UserConnectCommand command)
        {
            // Register the the connection (one connection per user only)
            connectionManager.Add(command.UserID, command.ConnectionID);
        }
        #endregion
    }
}