using Application.Features.Abstraction;
using Application.Features.Game.Commands;
using Application.Interfaces.Realtime.Managers;
using Application.Services.WorldService;
using Contract;
using Contract.DTO.Runtime.WorldDomain;
using Domain.DomainException;
using ResponseCode;

namespace Application.Features.Game.Handlers
{
    public class EnterHubHandler : IHandler<EnterHubCommand, RoomSpatialDTO>
    {
        #region Attributes
        private readonly ISessionManager sessionManager;
        private readonly WorldContext worldContext;
        private readonly RoomMigrationService roomMigrationService;
        #endregion

        #region Properties
        #endregion

        public EnterHubHandler(
            ISessionManager sessionManager,
            WorldContext worldContext,
            RoomMigrationService roomMigrationService)
        {
            this.sessionManager = sessionManager;
            this.worldContext = worldContext;
            this.roomMigrationService = roomMigrationService;
        }

        #region Methods
        public async Task<RoomSpatialDTO> Handle(
            EnterHubCommand command)
        {
            // Validate hud ids
            if (!Constraint.STATIC_HUB_ROOM_MAPS.Any(map => map.SpatialId == command.HubRoomSpatialID))
                throw new BadRequest(
                    ApplicationCode.GameHandlerCode.EnterHubInvalidHubRoom,
                    $"Hub room '{command.HubRoomSpatialID}' is not a registered static hub room.");

            // Validate player session existence
            var playerInstanceId = sessionManager.Get(command.UserID);
            if (playerInstanceId == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.EnterHubSessionNotFound,
                    $"Session missing for user ID: {command.UserID}");

            // Validate player runtime existence
            var player = worldContext.GetEntity(playerInstanceId);
            if (player == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.EnterHubPlayerInstanceNotFound,
                    $"Player runtime instance missing for ID: {playerInstanceId}");

            // Migrate player safely using calculated blueprint rules
            return await roomMigrationService.EnterRoomAsync(
                player: player,
                destinationRoomId: command.HubRoomSpatialID);
        }
        #endregion
    }
}