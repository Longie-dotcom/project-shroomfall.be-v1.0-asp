using Application.Context;
using Application.Features.Abstraction;
using Application.Features.Game.Commands;
using Application.Interfaces.Cache;
using Application.Interfaces.Realtime.Managers;
using Application.Services.WorldService;
using AutoMapper;
using Contract.DTO.Connection;
using Contract.DTO.Domain.Runtime;
using Contract.Enum.WorldDomain;
using Domain.Common;
using Domain.Runtime.EntityDomain.Component;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Application.Features.Game.Handlers
{
    public class EnterHubHandler : IHandler<EnterHubCommand, RoomSnapshotDTO>
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly ISessionManager sessionManager;
        private readonly WorldContext worldContext;
        private readonly ICacheProvider cacheProvider;
        private readonly ResidencyService residencyService;
        private readonly RoomMigrationService roomMigrationService;
        #endregion

        #region Properties
        #endregion

        public EnterHubHandler(
            IMapper mapper,
            ISessionManager sessionManager,
            WorldContext worldContext,
            ICacheProvider cacheProvider,
            ResidencyService residencyService,
            RoomMigrationService roomMigrationService)
        {
            this.mapper = mapper;
            this.sessionManager = sessionManager;
            this.worldContext = worldContext;
            this.cacheProvider = cacheProvider;
            this.residencyService = residencyService;
            this.roomMigrationService = roomMigrationService;
        }

        #region Methods
        public async Task<RoomSnapshotDTO> Handle(
            EnterHubCommand command)
        {
            // Validate player session and runtime instance state
            var playerInstanceId = sessionManager.Get(command.UserID);
            if (playerInstanceId == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.EnterHubSessionNotFound,
                    $"Session missing for user ID: {command.UserID}");

            var player = worldContext.GetEntity(playerInstanceId);
            if (player == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.EnterHubPlayerInstanceNotFound,
                    $"Player runtime instance missing for ID: {playerInstanceId}");

            var transform = player.GetComponent<TransformInstance>();
            if (transform == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.EnterHubTransformMissing,
                    $"Player instance {player.ID} is missing structural TransformInstance component");

            // Direct verification against your pre-booted room context
            var hubRoom = worldContext.GetRoom(command.HubRoomSpatialID);
            if (hubRoom == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.EnterHubRoomNotFound,
                    $"Critical Engine Failure: Public hub room spatial target '{command.HubRoomSpatialID}' was not initialized at server boot.");

            // Find hub room definition
            var roomDefinition = cacheProvider.Room.Get(hubRoom.DefinitionID);
            if (roomDefinition == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.EnterHubRoomDefinitionNotFound,
                    $"Public hub room spatial target '{command.HubRoomSpatialID}' has no definition");

            // Find player spawn
            var playerSpawnRule = roomDefinition.EntitySpawnRules.FirstOrDefault(r => r.Type == SpawnRuleType.Player);
            if (playerSpawnRule == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.EnterHubPlayerSpawnRuleMissing,
                    $"Map Configuration Error: Room Definition '{roomDefinition.ID}' does not contain a designated Player spawn rule layout configuration.");

            // Calculate coordinates (Using the exact point or center of the designated tile cell zone)
            int x = Random.Shared.Next(playerSpawnRule.MinX, playerSpawnRule.MaxX + 1);
            int y = Random.Shared.Next(playerSpawnRule.MinY, playerSpawnRule.MaxY + 1);
            
            var cell = cacheProvider.Room.GetTopCell(roomDefinition.ID, x, y);
            if (cell == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.EnterHubRoomNoSpawnCellFound,
                    $"Coordinate resolution failed. No valid cell architecture found at grid location ({x}, {y}) inside Room Blueprint '{roomDefinition.ID}'.");

            Vector2 dynamicHubSpawn = new Vector2(x, y);

            // Ensure segment arrays are hot in RAM
            var hubSnapshot = await residencyService.EnsureRoomLoaded(command.HubRoomSpatialID);

            // Migrate player safely using calculated blueprint rules
            await roomMigrationService.ExecuteMigrationAsync(
                player: player,
                transform: transform,
                toRoomSpatialId: command.HubRoomSpatialID,
                spawnPosition: dynamicHubSpawn,
                layerZ: cell.Z
            );

            return BuildDTO(hubSnapshot);
        }

        private RoomSnapshotDTO BuildDTO(RoomSnapshot snapshot)
        {
            var snapshotDto = new RoomSnapshotDTO()
            {
                RoomData = mapper.Map<RoomRuntimeDTO>(snapshot.Room)
            };

            snapshotDto.RoomData.Entities = mapper.Map<List<EntityInstanceDTO>>(snapshot.Entities);

            return snapshotDto;
        }
        #endregion
    }
}