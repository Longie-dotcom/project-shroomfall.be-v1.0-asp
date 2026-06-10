using Application.Context;
using Application.Features.Abstraction;
using Application.Features.Game.Commands;
using Application.Interfaces.Security;
using Application.Services.ItemService;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Shared;

namespace Application.Features.Game.Handlers
{
    public class UnequipItemHandler : IHandler<UnequipItemCommand>
    {
        #region Attributes
        private readonly WorldContext worldContext;
        private readonly ISessionManager sessionManager;
        private readonly ItemService itemService;
        #endregion

        #region Properties
        #endregion

        public UnequipItemHandler(
            WorldContext worldContext,
            ISessionManager sessionManager,
            ItemService itemService)
        {
            this.worldContext = worldContext;
            this.sessionManager = sessionManager;
            this.itemService = itemService;
        }

        #region Methods
        public async Task Handle(
            UnequipItemCommand command)
        {
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
            itemService.Unequip(player, command.Slot);
        }
        #endregion
    }
}