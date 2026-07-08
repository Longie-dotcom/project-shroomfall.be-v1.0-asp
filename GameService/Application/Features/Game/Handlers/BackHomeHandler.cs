using Application.Features.Abstraction;
using Application.Features.Game.Commands;
using Application.Interfaces.Realtime.Managers;
using Application.Services.WorldService;
using Contract.DTO.Connection;
using Domain.DomainException;
using Domain.Runtime.EntityDomain.Component;
using ResponseCode;

namespace Application.Features.Game.Handlers
{
    public class BackHomeHandler : IHandler<BackHomeCommand, RoomSnapshotDTO>
    {
        #region Attributes
        private readonly ISessionManager sessionManager;
        private readonly WorldContext worldContext;
        private readonly RoomMigrationService roomMigrationService;
        #endregion

        #region Properties
        #endregion

        public BackHomeHandler(
            ISessionManager sessionManager,
            WorldContext worldContext,
            RoomMigrationService roomMigrationService)
        {
            this.sessionManager = sessionManager;
            this.worldContext = worldContext;
            this.roomMigrationService = roomMigrationService;
        }

        #region Methods
        public async Task<RoomSnapshotDTO> Handle(
            BackHomeCommand command)
        {
            // Validate player session
            var playerInstanceId = sessionManager.Get(command.UserID);
            if (playerInstanceId == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.BackHomeSessionNotFound,
                    $"Session missing for user '{command.UserID}'.");

            // Validate runtime player
            var player = worldContext.GetEntity(playerInstanceId);
            if (player == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.BackHomePlayerInstanceNotFound,
                    $"Player runtime instance '{playerInstanceId}' not found.");

            // Resolve owned personal room
            var ownership = player.GetComponent<OwnershipInstance>();
            if (ownership == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.BackHomeOwnershipInstanceNotFound,
                    $"Player runtime instance '{playerInstanceId}' has no Ownership Instance.");

            // Migrate player safely using calculated blueprint rules
            return await roomMigrationService.EnterRoomAsync(
                player: player,
                destinationRoomId: ownership.PersonalRoomID);
        }
        #endregion
    }
}