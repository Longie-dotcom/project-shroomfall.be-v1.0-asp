using Application.Features.Abstraction;
using Application.Features.Game.Commands;
using Application.Interfaces.Realtime.Managers;
using Application.Services.WorldService;
using Domain.Common;
using Domain.DomainException;
using Domain.Runtime.EntityDomain.Component;
using ResponseCode;

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
        public async Task Handle(
            MoveCommand command)
        {
            var dto = command.DTO;

            // Validate session existence
            var playerInstanceId = sessionManager.Get(command.UserID);
            if (string.IsNullOrWhiteSpace(playerInstanceId))
                throw new Unauthorized(
                    ApplicationCode.GameHandlerCode.MoveSessionNotFound,
                    $"User with user ID: {command.UserID} has no session");

            // Validate player instance existence
            var player = worldContext.GetEntity(playerInstanceId);
            if (player == null)
                throw new BadRequest(
                    ApplicationCode.GameHandlerCode.MovePlayerInstanceNotFound,
                    $"User with user ID: {command.UserID} has no player instance");

            // Validate transform existence
            var transform = player.GetComponent<TransformInstance>();
            if (transform == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.MoveTransformMissing,
                    $"Player instance {playerInstanceId} is missing TransformInstance component");

            // Fire intent
            transform.SetMovementIntent(new Vector2(dto.X, dto.Y));
        }
        #endregion
    }
}