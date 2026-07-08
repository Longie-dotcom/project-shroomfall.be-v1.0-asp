using Application.Features.Abstraction;
using Application.Features.Game.Commands;
using Application.Interfaces.Realtime.Managers;
using Application.Services.ItemService;
using Application.Services.WorldService;
using Domain.DomainException;
using ResponseCode;

namespace Application.Features.Game.Handlers
{
    public class UnequipItemHandler : IHandler<UnequipItemCommand>
    {
        #region Attributes
        private readonly WorldContext worldContext;
        private readonly ISessionManager sessionManager;
        private readonly ItemUsageService itemUsageService;
        #endregion

        #region Properties
        #endregion

        public UnequipItemHandler(
            WorldContext worldContext,
            ISessionManager sessionManager,
            ItemUsageService itemUsageService)
        {
            this.worldContext = worldContext;
            this.sessionManager = sessionManager;
            this.itemUsageService = itemUsageService;
        }

        #region Methods
        public async Task Handle(
            UnequipItemCommand command)
        {
            // Validate session existence
            var playerInstanceId = sessionManager.Get(command.UserID);
            if (string.IsNullOrWhiteSpace(playerInstanceId))
                throw new Unauthorized(
                    ApplicationCode.GameHandlerCode.UnequipItemSessionNotFound,
                    $"User with user ID: {command.UserID} has no session");

            // Validate player instance existence
            var player = worldContext.GetEntity(playerInstanceId);
            if (player == null)
                throw new BadRequest(
                    ApplicationCode.GameHandlerCode.UnequipItemPlayerInstanceNotFound,
                    $"User with user ID: {command.UserID} has no player instance");

            // Fire intent
            itemUsageService.Unequip(player, command.Slot);
        }
        #endregion
    }
}