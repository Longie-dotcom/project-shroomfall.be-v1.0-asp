using Application.Features.Abstraction;
using Application.Features.Game.Commands;
using Application.Interfaces.Security;
using Domain.Abstraction.World;
using Domain.Common;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Shared;

namespace Application.Features.Game.Handlers
{
    public class MoveHandler : IHandler<MoveCommand>
    {
        #region Attributes
        private readonly IWorldQuery worldQuery;
        private readonly ISessionManager sessionManager;
        #endregion

        #region Properties
        #endregion

        public MoveHandler(
            IWorldQuery worldQuery,
            ISessionManager sessionManager)
        {
            this.worldQuery = worldQuery;
            this.sessionManager = sessionManager;
        }

        #region Methods
        public async Task Handle(MoveCommand command)
        {
            var dto = command.DTO;

            // Validate session existence
            var playerInstanceId = sessionManager.GetActivePlayer(command.UserID);
            if (string.IsNullOrWhiteSpace(playerInstanceId))
                throw new Unauthorized(
                    ResponseCode.Move_SessionNotFound,
                    $"User with user ID: {command.UserID} has no session");

            // Validate player instance existence
            var player = worldQuery.Get<PlayerInstance>(playerInstanceId);
            if (player == null)
                throw new BadRequest(
                    ResponseCode.Move_PlayerInstanceNotFound,
                    $"User with user ID: {command.UserID} has no player instance");

            // Fire intent
            player.SetMovementIntent(new Vector2(dto.X, dto.Y));
        }
        #endregion
    }
}