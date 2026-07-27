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

            // 1. Log receipt of command & complete DTO state
            Console.WriteLine($"[Server][UseItem] Received UseItemCommand from UserID: {command.UserID}");
            if (dto != null)
            {
                Console.WriteLine($"[Server][UseItem] DTO Details -> " +
                    $"ItemInstanceID: '{dto.ItemInstanceID}', " +
                    $"TargetPos: ({dto.TargetPositionX}, {dto.TargetPositionY}), " +
                    $"UnequippedSlot: {dto.UnequippedSlot}, " +
                    $"Action: {dto.ItemUsageAction}");
            }
            else
            {
                Console.WriteLine("[Server][UseItem] ERROR: Command DTO is NULL!");
            }

            // Validate session existence
            var playerInstanceId = sessionManager.Get(command.UserID);
            Console.WriteLine($"[Server][UseItem] Resolved PlayerInstanceID: '{playerInstanceId}'");

            if (string.IsNullOrWhiteSpace(playerInstanceId))
            {
                Console.WriteLine($"[Server][UseItem] REJECTED: UserID '{command.UserID}' has no active session.");
                throw new Unauthorized(
                    ApplicationCode.GameHandlerCode.UseItemSessionNotFound,
                    $"User with user ID: {command.UserID} has no session");
            }

            // Validate player instance existence
            var player = worldContext.GetEntity(playerInstanceId);
            if (player == null)
            {
                Console.WriteLine($"[Server][UseItem] REJECTED: Entity '{playerInstanceId}' not found in worldContext.");
                throw new BadRequest(
                    ApplicationCode.GameHandlerCode.UseItemPlayerInstanceNotFound,
                    $"User with user ID: {command.UserID} has no player instance");
            }

            // Validate action component existence
            var actionState = player.GetComponent<ActionInstance>();
            if (actionState == null)
            {
                Console.WriteLine($"[Server][UseItem] REJECTED: Player entity '{playerInstanceId}' missing ActionInstance component.");
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.UseItemActionComponentMissing,
                    $"Player instance {playerInstanceId} is missing ActionInstance component");
            }

            // Fire intent
            Console.WriteLine($"[Server][UseItem] SUCCESS -> Setting item use intent for ItemInstanceID: '{dto.ItemInstanceID}'");
            actionState.SetItemUseIntent(
                dto.ItemInstanceID,
                new Vector2(dto.TargetPositionX, dto.TargetPositionY),
                dto.UnequippedSlot,
                dto.ItemUsageAction);
        }
        #endregion
    }
}