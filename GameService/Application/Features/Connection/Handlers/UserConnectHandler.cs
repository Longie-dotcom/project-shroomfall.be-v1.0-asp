using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
using Application.Interfaces.Realtime;
using Domain.DomainException;
using Domain.Shared;

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

            // Validate connection existed
            var connection = connectionRegistry.Get(userId);
            if (connection != null)
                throw new BadRequest(
                    ResponseCode.UserConnect_ConnectionAlreadyExisted,
                    $"Connection already existed with user ID: {userId}");

            // Register the the connection
            connectionRegistry.Add(userId, connectionId);
        }
        #endregion
    }
}