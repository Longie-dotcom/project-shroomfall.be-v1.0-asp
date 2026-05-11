using Application.Interfaces.Cache;
using Application.Interfaces.Factory;
using Application.Services.Abstraction.WorldService;
using Domain.Common;
using Domain.Definition.EntityDomain;
using Domain.Definition.WorldDomain.Enum;
using Domain.DomainException;
using Domain.Shared;

namespace Application.Services.WorldService
{
    public class CreationService : ICreationService
    {
        #region Attributes
        private readonly IEntityCache entityCache;
        private readonly IRoomSpatialFactory roomSpatialFactory;
        private readonly IPlayerInstanceFactory playerInstanceFactory;
        private readonly IWorldObjectInstanceFactory worldObjectInstanceFactory;
        private readonly IInitializationService initializationService;
        private readonly IWorldExpansionService worldExpansionService;
        private readonly ISpawnService spawnService;
        #endregion

        #region Properties
        #endregion

        public CreationService(
            IEntityCache entityCache,
            IRoomSpatialFactory roomSpatialFactory,
            IPlayerInstanceFactory playerInstanceFactory,
            IWorldObjectInstanceFactory worldObjectInstanceFactory,
            IInitializationService initializationService,
            IWorldExpansionService worldExpansionService,
            ISpawnService spawnService)
        {
            this.entityCache = entityCache;
            this.roomSpatialFactory = roomSpatialFactory;
            this.playerInstanceFactory = playerInstanceFactory;
            this.worldObjectInstanceFactory = worldObjectInstanceFactory;
            this.initializationService = initializationService;
            this.worldExpansionService = worldExpansionService;
            this.spawnService = spawnService;
        }

        #region Methods
        public WorldContext CreatePlayerContext(
            string playerDefinitionId,
            string roomDefinitionId,
            string userId)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var roomSpatialId = $"PLAYER_ROOM_{userId}_{timestamp}";
            var playerInstanceId = $"PLAYER_{userId}_{timestamp}";

            var room = roomSpatialFactory.Create(
                definitionId: roomDefinitionId,
                instanceId: roomSpatialId,
                ownerId: playerInstanceId);

            var seedContext = initializationService.InitializeRoomEnvironment(
                roomSpatialId: room.ID,
                roomDefinitionId: room.DefinitionID
            );

            var context = worldExpansionService.Expand(seedContext);

            var (spawnPosition, layerZ) = spawnService.ResolveSpawnPosition(
                room.DefinitionID,
                playerDefinitionId,
                SpawnRuleType.Player);

            var player = playerInstanceFactory.Create(
                definitionId: playerDefinitionId,
                instanceId: playerInstanceId,
                userId: userId,
                roomSpatialId: roomSpatialId,
                layerZ: layerZ,
                position: spawnPosition,
                direction: new Vector2(0, 1)
            );

            context.Entities.Add(player);
            context.Rooms.Add(room);

            return context;
        }

        public WorldContext CreatePlacedWorldObjectContext(
            string worldObjectDefinitionId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 direction)
        {
            var context = new WorldContext();
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var worldObjectInstanceId = $"WORLD_OBJECT_{timestamp}";

            var (worldObject, linkedRoomSpatialId) = worldObjectInstanceFactory.Create(
                definitionId: worldObjectDefinitionId,
                instanceId: worldObjectInstanceId,
                roomSpatialId: roomSpatialId,
                layerZ: layerZ,
                position: position,
                direction: direction);

            context.Entities.Add(worldObject);

            if (!string.IsNullOrWhiteSpace(linkedRoomSpatialId))
            {
                var worldObjectDef = entityCache.Get<WorldObject>(worldObjectDefinitionId);

                if (worldObjectDef == null)
                    throw new InternalException(ResponseCode.InitializationService_WorldObjectDefinitionNotFound);

                context.PendingRooms.Add(new PendingRoomInitialization
                {
                    RoomSpatialID = linkedRoomSpatialId,
                    RoomDefinitionID = worldObjectDef.RoomID!
                });
            }

            return worldExpansionService.Expand(context);
        }
        #endregion
    }
}