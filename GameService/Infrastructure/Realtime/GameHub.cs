using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
using Application.Features.Game.Commands;
using Contract;
using Contract.DTO.Feature.Game.Command;
using Contract.Enum.IdentityDomain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Realtime
{
    //[Authorize(Roles = nameof(Role.Player) + "," + nameof(Role.Admin) + "," + nameof(Role.Designer))]
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
            var (userId, connectionId) = HubContextValidator.GetValidatedContext(this);

            await dispatcher.Send<UserConnectCommand>(
                new UserConnectCommand(userId, connectionId)
            );

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(
            Exception? exception)
        {
            var (userId, connectionId) = HubContextValidator.GetValidatedContext(this);

            await dispatcher.Send<UnloadSessionCommand>(
                new UnloadSessionCommand(userId, connectionId)
            );

            await base.OnDisconnectedAsync(exception);
        }

        [HubMethodName(NetworkMethod.Move)]
        public async Task Move(
            MoveDTO dto)
        {
            var (userId, connectionId) = HubContextValidator.GetValidatedContext(this);

            await dispatcher.Send<MoveCommand>(
                new MoveCommand(userId, dto)
            );
        }
        #endregion
    }
}