using Application.Interfaces.Cache;
using Application.Interfaces.Realtime.Managers;
using AutoMapper;
using Contract.DTO.Runtime.EntityDomain;
using Contract.DTO.Runtime.WorldDomain;
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
        private readonly CollisionService collisionService;
        #endregion

        public RoomMigrationService(
            IMapper mapper,
            ResidencyService residencyService,
            EntitySpawnService entitySpawnService,
            IConnectionManager connectionManager,
            ICacheProvider cacheProvider,
            CollisionService collisionService)
        {
            this.mapper = mapper;
            this.residencyService = residencyService;
            this.entitySpawnService = entitySpawnService;
            this.connectionManager = connectionManager;
            this.cacheProvider = cacheProvider;
            this.collisionService = collisionService;
        }

        #region Methods
        public async Task<RoomSpatialDTO> EnterRoomAsync(
            EntityInstance player,
            string destinationRoomId)
        {
            var transform = player.GetComponent<TransformInstance>();
            if (transform == null)
                throw new InternalException(
                    ApplicationCode.RoomMigrationServiceCode.TransformMissing,
                    $"Entity '{player.ID}' has no TransformInstance.");

            // Bring target room into memory
            var roomSnapshot = await residencyService.EnsureRoomLoaded(destinationRoomId);

            // Resolve valid spawn by rules
            var (spawnPosition, layerZ) = ResolvePlayerSpawn(roomSnapshot.Room, player, transform, roomSnapshot.Entities);

            // Restore old room id
            var oldRoomId = transform.RoomSpatialID;

            // Standard live gameplay room-to-room migration (works perfectly for same-room teleports too!)
            entitySpawnService.TransitionRoom(player, destinationRoomId, spawnPosition, layerZ);

            // Only leave old groups/leases if we ACTUALLY changed rooms
            if (oldRoomId != destinationRoomId)
            {
                await residencyService.PlayerLeaveRoomAsync(oldRoomId, player.ID);
            }

            // Update ongoing tracking layers (Safe to call repeatedly, it uses HashSet)
            await residencyService.PlayerJoinRoomAsync(destinationRoomId, player.ID);

            var ownership = player.GetComponent<OwnershipInstance>();
            if (ownership != null)
            {
                var connectionId = connectionManager.Get(ownership.UserID);
                if (connectionId != null)
                {
                    if (oldRoomId != destinationRoomId)
                        await connectionManager.Ungroup(connectionId, oldRoomId);

                    await connectionManager.Group(connectionId, destinationRoomId);
                }
            }

            return BuildDTO(roomSnapshot);
        }

        public async Task PlayerQuitGame(
            string roomId,
            string userId,
            string connectionId,
            EntityInstance player)
        {
            // Freeze active engine loops first
            entitySpawnService.Despawn(player);

            // Sever this specific connection from SignalR updates and clear it from our tracking state
            await connectionManager.Ungroup(connectionId, roomId);
            connectionManager.Remove(userId, connectionId);

            // Release the room residency lease
            await residencyService.PlayerQuitGame(roomId, player);
        }

        private (Vector2 position, int layerZ) ResolvePlayerSpawn(
            RoomSpatial room,
            EntityInstance player,
            TransformInstance transform,
            IEnumerable<EntityInstance> roomEntities)
        {
            var roomDefinition = cacheProvider.Room.Get(room.DefinitionID);
            if (roomDefinition == null)
                throw new InternalException(
                    ApplicationCode.RoomMigrationServiceCode.RoomDefinitionNotFound,
                    $"Room spatial '{room.ID}' references unknown room definition '{room.DefinitionID}'.");

            var playerSpawnRule = roomDefinition.EntitySpawnRules.FirstOrDefault(r => r.Type == SpawnRuleType.Player);
            if (playerSpawnRule == null)
                throw new InternalException(
                    ApplicationCode.RoomMigrationServiceCode.PlayerSpawnRuleMissing,
                    $"Room definition '{roomDefinition.ID}' does not define a player spawn rule.");

            int x = Random.Shared.Next(playerSpawnRule.MinX, playerSpawnRule.MaxX + 1);
            int y = Random.Shared.Next(playerSpawnRule.MinY, playerSpawnRule.MaxY + 1);

            var cell = cacheProvider.Room.GetTopCell(roomDefinition.ID, x, y);
            if (cell == null)
                throw new InternalException(
                    ApplicationCode.RoomMigrationServiceCode.SpawnCellNotFound,
                    $"No valid spawn cell exists at ({x}, {y}) in room definition '{roomDefinition.ID}'.");

            var initialPosition = new Vector2(x, y);
            int initialLayerZ = cell.Z;

            var collisionInstance = player.GetComponent<CollisionInstance>();
            if (collisionInstance != null)
            {
                var collisionBody = new CollisionBody(
                    player.ID,
                    room.ID,
                    initialPosition,
                    collisionInstance.CollisionOffset,
                    initialLayerZ,
                    collisionInstance.CollisionShape,
                    collisionInstance.Layer,
                    collisionInstance.Mask);

                // Pass the real transform instance so collisionService can set the valid position directly
                collisionService.SpawnAtNearestValidPosition(
                    collisionBody,
                    transform,
                    room.DefinitionID,
                    roomEntities);

                return (transform.Position, transform.LayerZ);
            }

            return (initialPosition, initialLayerZ);
        }

        private RoomSpatialDTO BuildDTO(
            RoomInstance roomInstance)
        {
            var snapshotDto = mapper.Map<RoomSpatialDTO>(roomInstance.Room);
            snapshotDto.Entities = mapper.Map<List<EntityInstanceDTO>>(roomInstance.Entities);
            return snapshotDto;
        }
        #endregion
    }
}