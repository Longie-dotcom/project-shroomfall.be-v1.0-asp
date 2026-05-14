using Application.Context;
using Application.Features.Abstraction;
using Application.Features.Game.Commands;
using Application.Interfaces.Security;
using Domain.Common;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Shared;

namespace Application.Features.Game.Handlers
{
    public class MoveHandler : IHandler<MoveCommand>
    {
        #region Attributes
        private readonly WorldContext worldContext;
        private readonly ISessionManager sessionManager;
        #endregion

        #region Properties
        #endregion

        public MoveHandler(
            WorldContext worldContext,
            ISessionManager sessionManager)
        {
            this.worldContext = worldContext;
            this.sessionManager = sessionManager;
        }

        #region Methods
        public async Task Handle(MoveCommand command)
        {
            var dto = command.DTO;

            // Validate session existence
            var playerInstanceId = sessionManager.Get(command.UserID);
            if (string.IsNullOrWhiteSpace(playerInstanceId))
                throw new Unauthorized(
                    ResponseCode.Move_SessionNotFound,
                    $"User with user ID: {command.UserID} has no session");

            // Validate player instance existence
            var player = worldContext.GetEntity<PlayerInstance>(playerInstanceId);
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