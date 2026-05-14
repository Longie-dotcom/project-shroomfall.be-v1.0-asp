using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
using Application.Interfaces.Realtime;

namespace Application.Features.Connection.Handlers
{
    public class UserConnectHandler : IHandler<UserConnectCommand>
    {
        #region Attributes
        private readonly IConnectionRegistry connectionRegistry;
        #endregion

        #region Properties
        #endregion

        public UserConnectHandler(
            IConnectionRegistry connectionRegistry)
        {
            this.connectionRegistry = connectionRegistry;
        }

        #region Methods
        public async Task Handle(
            UserConnectCommand command)
        {
            var userId = command.UserID;
            var connectionId = command.ConnectionID;

            // Register the the connection (allow multiple connection per user)
            connectionRegistry.Add(userId, connectionId);
        }
        #endregion
    }
}