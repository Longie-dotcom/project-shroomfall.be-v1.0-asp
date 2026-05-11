using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
using Infrastructure.Helper;
using Microsoft.AspNetCore.SignalR;

namespace SignalHub
{
    public class GameHub : Hub
    {
        #region Attributes
        private readonly IDispatcher dispatcher;
        #endregion

        #region Properties
        #endregion

        public GameHub(
            IDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        #region Methods
        public override async Task OnConnectedAsync()
        {
            var (userId, connectionId) = HubContextHelper.GetValidatedContext(this);

            await dispatcher.Send<UserConnectCommand>(
                new UserConnectCommand(userId, connectionId)
            );

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(
            Exception? exception)
        {
            var (userId, _) = HubContextHelper.GetValidatedContext(this);

            await dispatcher.Send<UnloadSessionCommand>(
                new UnloadSessionCommand(userId)
            );

            await base.OnDisconnectedAsync(exception);
        }
        #endregion
    }
}