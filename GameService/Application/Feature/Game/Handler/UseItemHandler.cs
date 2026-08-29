using Application.Feature.Abstraction;
using Application.Feature.Game.Command;
using Application.Interface.Realtime.Managers;
using Application.Service.WorldService;
using Contract.Common;
using Domain.DomainException;
using Domain.Runtime.EntityDomain.Component;
using ResponseCode;

namespace Application.Feature.Game.Handler
{
    public class UseItemHandler : IHandler<UseItemCommand>
    {
        #region Attributes
        private readonly WorldContext worldContext;
        private readonly ISessionManager sessionManager;
        #endregion

        #region Properties
        #endregion

        public UseItemHandler(
            WorldContext worldContext,
            ISessionManager sessionManager)
        {
            this.worldContext = worldContext;
            this.sessionManager = sessionManager;
        }

        #region Methods
        public async Task Handle(
            UseItemCommand command)
        {
            var dto = command.DTO;

            // Validate session existence
            var playerInstanceId = sessionManager.Get(command.UserID);
            if (string.IsNullOrWhiteSpace(playerInstanceId))
                throw new Unauthorized(
                    ApplicationCode.GameHandlerCode.UseItemSessionNotFound,
                    $"User with user ID: {command.UserID} has no session");

            // Validate player instance existence
            var player = worldContext.GetEntity(playerInstanceId);
            if (player == null)
                throw new BadRequest(
                    ApplicationCode.GameHandlerCode.UseItemPlayerInstanceNotFound,
                    $"User with user ID: {command.UserID} has no player instance");

            // Validate action component existence
            var actionState = player.GetComponent<ActionInstance>();
            if (actionState == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.UseItemActionComponentMissing,
                    $"Player instance {playerInstanceId} is missing ActionInstance component");

            // Fire intent
            actionState.SetItemUseIntent(
                dto.ItemInstanceID,
                new Vector2(dto.TargetPositionX, dto.TargetPositionY),
                dto.UnequippedSlot,
                dto.ItemUsageAction);
        }
        #endregion
    }
}