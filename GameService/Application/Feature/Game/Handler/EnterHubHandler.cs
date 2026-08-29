using Application.Feature.Abstraction;
using Application.Feature.Game.Command;
using Application.Interface.Realtime.Managers;
using Application.Service.WorldService;
using AutoMapper;
using Contract;
using Contract.DTO.Feature.Connection.Response;
using Contract.DTO.Runtime.EntityDomain;
using Contract.DTO.Runtime.WorldDomain;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using ResponseCode;

namespace Application.Feature.Game.Handler
{
    public class EnterHubHandler : IHandler<EnterHubCommand, SaveGameDTO>
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly ISessionManager sessionManager;
        private readonly WorldContext worldContext;
        private readonly RoomMigrationService roomMigrationService;
        #endregion

        #region Properties
        #endregion

        public EnterHubHandler(
            IMapper mapper,
            ISessionManager sessionManager,
            WorldContext worldContext,
            RoomMigrationService roomMigrationService)
        {
            this.mapper = mapper;
            this.sessionManager = sessionManager;
            this.worldContext = worldContext;
            this.roomMigrationService = roomMigrationService;
        }

        #region Methods
        public async Task<SaveGameDTO> Handle(
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
            var roomInstance = await roomMigrationService.EnterRoomAsync(
                player: player,
                destinationRoomId: command.HubRoomSpatialID);

            return BuildSaveGame(player, roomInstance);
        }

        private SaveGameDTO BuildSaveGame(
            EntityInstance player,
            RoomInstanceDTO room)
        {
            var saveGame = new SaveGameDTO()
            {
                PlayerData = mapper.Map<EntityInstanceDTO>(player),
                RoomData = room
            };

            return saveGame;
        }
        #endregion
    }
}