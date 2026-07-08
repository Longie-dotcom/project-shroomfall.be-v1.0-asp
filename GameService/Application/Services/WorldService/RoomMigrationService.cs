using Application.Interfaces.Cache;
using Application.Interfaces.Realtime.Managers;
using AutoMapper;
using Contract.DTO.Connection;
using Contract.DTO.Domain.Runtime;
using Contract.Enum.WorldDomain;
using Domain.Common;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.WorldDomain.Spatial;
using ResponseCode;

namespace Application.Services.WorldService
{
    public class RoomMigrationService
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly ResidencyService residencyService;
        private readonly EntitySpawnService entitySpawnService;
        private readonly IConnectionManager connectionManager;
        private readonly ICacheProvider cacheProvider;
        #endregion

        public RoomMigrationService(
            IMapper mapper,
            ResidencyService residencyService,
            EntitySpawnService entitySpawnService,
            IConnectionManager connectionManager,
            ICacheProvider cacheProvider)
        {
            this.mapper = mapper;
            this.residencyService = residencyService;
            this.entitySpawnService = entitySpawnService;
            this.connectionManager = connectionManager;
            this.cacheProvider = cacheProvider;
        }

        #region Methods
        public async Task<RoomSnapshotDTO> EnterRoomAsync(
            EntityInstance player,
            string destinationRoomId,
            bool isInitialLogin = false)
        {
            var transform = player.GetComponent<TransformInstance>();
            if (transform == null)
                throw new InternalException(
                    ApplicationCode.RoomMigrationServiceCode.TransformMissing,
                    $"Entity '{player.ID}' has no TransformInstance.");

            // Bring target room into memory
            var roomSnapshot = await residencyService.EnsureRoomLoaded(destinationRoomId);

            // Resolve valid spawn by rules
            var (spawnPosition, layerZ) = ResolvePlayerSpawn(roomSnapshot.Room);
            
            // Restore old room id
            var oldRoomId = transform.RoomSpatialID;

            if (isInitialLogin)
            {
                // Use login indexing path
                entitySpawnService.SpawnOnLogin(player, destinationRoomId, spawnPosition, layerZ);
            }
            else
            {
                // Standard live gameplay room-to-room migration
                entitySpawnService.TransitionRoom(player, destinationRoomId, spawnPosition, layerZ);
                await residencyService.LeaveRoomAsync(oldRoomId, player.ID);
            }

            // Update ongoing tracking layers
            await residencyService.JoinRoomAsync(destinationRoomId, player.ID);

            // Move groups
            var ownership = player.GetComponent<OwnershipInstance>();
            if (ownership != null)
            {
                foreach (var connectionId in connectionManager.Get(ownership.UserID))
                {
                    if (!isInitialLogin)
                        await connectionManager.Ungroup(connectionId, oldRoomId);
                    await connectionManager.Group(connectionId, destinationRoomId);
                }
            }

            return BuildDTO(roomSnapshot);
        }

        private (Vector2 position, int layerZ) ResolvePlayerSpawn(
            RoomSpatial room)
        {
            // Find hub room definition
            var roomDefinition = cacheProvider.Room.Get(room.DefinitionID);
            if (roomDefinition == null)
                throw new InternalException(
                    ApplicationCode.RoomMigrationServiceCode.RoomDefinitionNotFound,
                    $"Room spatial '{room.ID}' references unknown room definition '{room.DefinitionID}'.");

            // Find player spawn
            var playerSpawnRule = roomDefinition.EntitySpawnRules.FirstOrDefault(r => r.Type == SpawnRuleType.Player);
            if (playerSpawnRule == null)
                throw new InternalException(
                    ApplicationCode.RoomMigrationServiceCode.PlayerSpawnRuleMissing,
                    $"Room definition '{roomDefinition.ID}' does not define a player spawn rule.");

            // Calculate coordinates (Using the exact point or center of the designated tile cell zone)
            int x = Random.Shared.Next(playerSpawnRule.MinX, playerSpawnRule.MaxX + 1);
            int y = Random.Shared.Next(playerSpawnRule.MinY, playerSpawnRule.MaxY + 1);

            var cell = cacheProvider.Room.GetTopCell(roomDefinition.ID, x, y);
            if (cell == null)
                throw new InternalException(
                    ApplicationCode.RoomMigrationServiceCode.SpawnCellNotFound,
                    $"No valid spawn cell exists at ({x}, {y}) in room definition '{roomDefinition.ID}'.");

            return (new Vector2(x, y), cell.Z);
        }

        private RoomSnapshotDTO BuildDTO(
            RoomSnapshot snapshot)
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