using Application.Interfaces.Factory;
using Domain.Common;
using Domain.Definition.WorldDomain.Enum;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.WorldDomain;

namespace Application.Services.WorldService
{
    public class WorldGraph
    {
        // Runtime entities to inject into world
        public List<EntityInstance> Entities { get; set; } = new List<EntityInstance>();

        // Runtime rooms to inject into world
        public List<RoomSpatial> Rooms { get; set; } = new List<RoomSpatial>();

        // Deferred room expansions
        public List<PendingRoomInitialization> PendingRooms { get; set; } = new List<PendingRoomInitialization>();
    }

    public class PendingRoomInitialization
    {
        public string RoomSpatialID { get; set; } = string.Empty;
        public string RoomDefinitionID { get; set; } = string.Empty;
    }

    public class PlayerCreation : WorldGraph
    {
        public required PlayerInstance Player { get; init; }
        public required RoomSpatial Room { get; init; }
    }

    public class WorldObjectCreation : WorldGraph
    {
        public required WorldObjectInstance WorldObject { get; init; }
    }

    public class CreationService
    {
        #region Attributes
        private readonly IRoomSpatialFactory roomSpatialFactory;
        private readonly IPlayerInstanceFactory playerInstanceFactory;
        private readonly IWorldObjectInstanceFactory worldObjectInstanceFactory;
        private readonly InitializationService initializationService;
        private readonly WorldExpansionService worldExpansionService;
        private readonly SpawnService spawnService;
        #endregion

        #region Properties
        #endregion

        public CreationService(
            IRoomSpatialFactory roomSpatialFactory,
            IPlayerInstanceFactory playerInstanceFactory,
            IWorldObjectInstanceFactory worldObjectInstanceFactory,
            InitializationService initializationService,
            WorldExpansionService worldExpansionService,
            SpawnService spawnService)
        {
            this.roomSpatialFactory = roomSpatialFactory;
            this.playerInstanceFactory = playerInstanceFactory;
            this.worldObjectInstanceFactory = worldObjectInstanceFactory;
            this.initializationService = initializationService;
            this.worldExpansionService = worldExpansionService;
            this.spawnService = spawnService;
        }

        #region Methods
        public PlayerCreation CreatePlayer(
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
                roomDefinitionId: room.DefinitionID);

            var worldGraph = new WorldGraph
            {
                Entities = seedContext.Entities,
                Rooms = seedContext.Rooms,
                PendingRooms = seedContext.PendingRooms
            };

            worldGraph = worldExpansionService.Expand(worldGraph);

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
                direction: new Vector2(0, 1));

            worldGraph.Entities.Add(player);
            worldGraph.Rooms.Add(room);

            return new PlayerCreation
            {
                Player = player,
                Room = room,
                Entities = worldGraph.Entities,
                Rooms = worldGraph.Rooms,
                PendingRooms = worldGraph.PendingRooms
            };
        }

        public WorldObjectCreation CreateWorldObject(
            string worldObjectDefinitionId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 direction)
        {
            var worldGraph = new WorldGraph();
            var worldObjectInstanceId = $"WORLD_OBJECT_{Guid.NewGuid():N}";

            var (worldObject, linkedRoomSpatialId, linkedRoomDefinitionId) =
                worldObjectInstanceFactory.Create(
                    definitionId: worldObjectDefinitionId,
                    instanceId: worldObjectInstanceId,
                    roomSpatialId: roomSpatialId,
                    layerZ: layerZ,
                    position: position,
                    direction: direction);

            worldGraph.Entities.Add(worldObject);

            if (!string.IsNullOrWhiteSpace(linkedRoomSpatialId) 
                && !string.IsNullOrWhiteSpace(linkedRoomDefinitionId))
            {
                worldGraph.PendingRooms.Add(new PendingRoomInitialization
                {
                    RoomSpatialID = linkedRoomSpatialId,
                    RoomDefinitionID = linkedRoomDefinitionId
                });
            }

            worldGraph = worldExpansionService.Expand(worldGraph);

            return new WorldObjectCreation
            {
                WorldObject = worldObject,
                Entities = worldGraph.Entities,
                Rooms = worldGraph.Rooms,
                PendingRooms = worldGraph.PendingRooms
            };
        }
        #endregion
    }
}