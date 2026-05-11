using Application.Services.Abstraction.WorldService;
using Domain.Abstraction.World;
using Domain.Common;
using Domain.Definition.WorldDomain.Enum;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Shared;

namespace Application.Services.WorldService
{
    public class OrchestratorService : IOrchestratorService
    {
        #region Attributes
        private readonly IWorldQuery worldQuery;
        private readonly ICreationService creationService;
        private readonly ISaveService saveService;
        private readonly IContextService contextService;
        private readonly ISpawnService spawnService;
        private readonly ICollisionService collisionService;
        private readonly IWorldExpansionService worldExpansionService;
        #endregion

        #region Properties
        #endregion

        public OrchestratorService(
            IWorldQuery worldQuery,
            ICreationService creationService,
            ISaveService saveService,
            IContextService contextService,
            ISpawnService spawnService,
            ICollisionService collisionService,
            IWorldExpansionService worldExpansionService)
        {
            this.worldQuery = worldQuery;
            this.creationService = creationService;
            this.saveService = saveService;
            this.contextService = contextService;
            this.spawnService = spawnService;
            this.collisionService = collisionService;
            this.worldExpansionService = worldExpansionService;
        }

        #region Methods
        public async Task SpawnNewPlayer(
            string playerDefinitionId,
            string roomDefinitionId,
            string userId)
        {
            var context = creationService.CreatePlayerContext(
                playerDefinitionId: playerDefinitionId,
                roomDefinitionId: roomDefinitionId,
                userId: userId);

            var tx = new WorldTransaction
            {
                Context = context
            };

            ExpandWithRetry(tx);

            if (!tx.IsValid)
                throw new InvalidOperationException("World expansion incomplete.");

            await saveService.SaveWorldAsync(tx.Context);
        }

        public void SpawnPlacedWorldObject(
            string worldObjectDefinitionId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 direction)
        {
            var context = creationService.CreatePlacedWorldObjectContext(
                worldObjectDefinitionId,
                roomSpatialId,
                layerZ,
                position,
                direction);

            var worldObject = context.Entities.First();

            var collision = collisionService.QueryPoint(
                shape: worldObject.CollisionShape,
                roomSpatialId: worldObject.RoomSpatialID,
                position: worldObject.Position,
                layerZ: worldObject.LayerZ);

            if (collision.IsBlocked)
                throw new BadRequest(ResponseCode.OrchestratorService_PositionBlocked);

            var tx = new WorldTransaction
            {
                Context = context
            };

            ExpandWithRetry(tx);

            if (!tx.IsValid)
                throw new InvalidOperationException("World expansion incomplete.");

            contextService.AddEntity(worldObject);
        }

        public async Task LoadExistedPlayer(
            string playerInstanceId)
        {
            var player = await saveService.LoadPlayerAsync(playerInstanceId);
            if (player == null)
                throw new InternalException(
                    ResponseCode.OrchestratorService_PlayerInstanceOnLoadNotFound,
                    $"Player instance on load with instance ID: {playerInstanceId} is not found");

            var snapshot = await saveService.LoadRoomSnapshotAsync(player.RoomSpatialID);
            if (snapshot == null)
                throw new InternalException(
                    ResponseCode.OrchestratorService_RoomSnapshotOnLoadNotFound,
                    $"Room snapshot on load with spatial ID: {player.RoomSpatialID} is not found");

            contextService.AddEntity(player);
            contextService.LoadRoom(snapshot, playerInstanceId);
        }

        public async Task UnloadExistedPlayer(
            string playerInstanceId)
        {
            var player = worldQuery.Get<PlayerInstance>(playerInstanceId);
            if (player == null)
                return;

            var snapshot = contextService.UnloadRoom(player.RoomSpatialID);
            if (snapshot != null)
                await saveService.SaveRoomAsync(snapshot);
        }

        public async Task EntityChangeRoom(
            string entityInstanceId,
            string targetRoomId)
        {
            var entity = worldQuery.Get<EntityInstance>(entityInstanceId);

            if (entity == null)
                throw new BadRequest("Entity not found");

            var targetRoom = await saveService.LoadRoomSnapshotAsync(targetRoomId);

            if (targetRoom == null)
                throw new Exception("Target room not found");

            var (position, layerZ) = spawnService.ResolveSpawnPosition(
                targetRoom.Room.DefinitionID,
                entity.DefinitionID,
                SpawnRuleType.Player);

            var collision = collisionService.QueryPoint(
                shape: entity.CollisionShape,
                roomSpatialId: targetRoomId,
                position: position,
                layerZ: layerZ);

            if (collision.IsBlocked)
                throw new BadRequest("Position blocked");

            var fromRoomId = entity.RoomSpatialID;

            entity.ChangeRoom(targetRoomId, position, layerZ);

            contextService.ChangeRoom(
                entityInstanceId,
                fromRoomId,
                targetRoom);

            await saveService.SaveEntityAsync(entity);
        }

        private void ExpandWithRetry(
            WorldTransaction tx)
        {
            const int maxRetries = 3;

            for (int i = 0; i < maxRetries; i++)
            {
                tx.Context = worldExpansionService.Expand(tx.Context);

                if (!tx.Context.PendingRooms.Any())
                {
                    tx.IsExpanded = true;
                    return;
                }
            }

            throw new InternalException("World expansion failed after retries.");
        }
        #endregion
    }
}