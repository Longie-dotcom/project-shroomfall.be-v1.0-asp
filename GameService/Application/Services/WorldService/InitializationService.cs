using Application.Interfaces.Cache;
using Application.Services.WorldService.Factory;
using Contract.Enum.WorldDomain;
using Domain.Common;
using Domain.Definition.WorldDomain;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.WorldDomain.Spatial;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Application.Services.WorldService
{
    public class RoomSnapshot
    {
        public RoomSpatial Room { get; set; } = default!;
        public List<EntityInstance> Entities { get; set; } = new List<EntityInstance>();
    }

    public class InitializationService
    {
        #region Attributes
        private readonly Random random;
        private readonly ICacheProvider cacheProvider;  
        private readonly CollisionService collisionService;
        private readonly EntityInstanceFactory entityInstanceFactory;
        private readonly RoomSpatialFactory roomSpatialFactory;
        #endregion

        #region Properties
        #endregion

        public InitializationService(
            ICacheProvider cacheProvider,
            CollisionService collisionService,
            EntityInstanceFactory entityInstanceFactory,
            RoomSpatialFactory roomSpatialFactory)
        {
            random = new Random();
            this.cacheProvider = cacheProvider;
            this.collisionService = collisionService;
            this.entityInstanceFactory = entityInstanceFactory;
            this.roomSpatialFactory = roomSpatialFactory;
        }

        #region Methods
        public (RoomSnapshot room, EntityInstance? player) InitializeRoom(
            string roomDefinitionId,
            string roomSpatialId,
            string? playerDefinitionId,
            string? playerInstanceId,
            string? userId = null)
        {
            var room = roomSpatialFactory.Create(roomDefinitionId, roomSpatialId, playerInstanceId);
            var pendingEntities = new List<EntityInstance>();
            EntityInstance? player = null; 

            // Spawn Context-Specific (Player) Entities
            if (!string.IsNullOrEmpty(playerDefinitionId) &&
                !string.IsNullOrEmpty(playerInstanceId) &&
                !string.IsNullOrEmpty(userId))
            {
                player = SpawnEntitiesByType(
                    roomDefinitionId, 
                    roomSpatialId, 
                    SpawnRuleType.Player, 
                    pendingEntities,
                    entityDefId: playerDefinitionId,
                    forcedInstanceId: playerInstanceId,
                    userId: userId);
            }

            // Spawn Environmental Entities
            SpawnEntitiesByType(roomDefinitionId, roomSpatialId, SpawnRuleType.Environment, pendingEntities);

            return (new RoomSnapshot { Room = room, Entities = pendingEntities }, player);
        }

        private EntityInstance? SpawnEntitiesByType(
            string roomDefId,
            string roomSpatialId,
            SpawnRuleType type,
            List<EntityInstance> buffer,
            string? entityDefId = null,
            string? forcedInstanceId = null,
            string? userId = null)
        {
            var roomDef = cacheProvider.Room.Get(roomDefId);
            if (roomDef == null)
                throw new InternalException(
                    ApplicationCode.InitializationServiceCode.RoomDefinitionNotFound,
                    $"Room generation aborted. Master definition blueprint for ID '{roomDefId}' could not be loaded from store.");

            var rules = roomDef.EntitySpawnRules.Where(r => r.Type == type);
            if (!string.IsNullOrEmpty(entityDefId)) rules = rules.Where(r => r.EntityDefinitionID == entityDefId);

            EntityInstance? lastSpawned = null;

            foreach (var rule in rules)
            {
                int count = random.Next(rule.MinCount, rule.MaxCount + 1);
                for (int i = 0; i < count; i++)
                {
                    var (pos, layerZ) = ResolveSpawnPosition(roomDef, rule.EntityDefinitionID, type);

                    string instanceId = forcedInstanceId ?? $"{Guid.NewGuid()}_{rule.EntityDefinitionID}";

                    // ─────────────────────────────
                    // Context switching logic
                    // ─────────────────────────────
                    WorldEntityCreateContext context;

                    if (type == SpawnRuleType.Player && !string.IsNullOrEmpty(userId))
                    {
                        context = new PlayerEntityCreateContext(
                            instanceId, rule.EntityDefinitionID, roomSpatialId, layerZ, pos, userId);
                    }
                    else
                    {
                        context = new WorldEntityCreateContext(
                            instanceId, rule.EntityDefinitionID, roomSpatialId, layerZ, pos);
                    }

                    // Factory handles the specific context via polymorphism
                    var entity = entityInstanceFactory.Create(context);
                    
                    collisionService.SpawnAtNearestValidPosition(entity, roomDef.ID, roomSpatialId, pos, layerZ, buffer, 5);
                    buffer.Add(entity);

                    lastSpawned = entity;
                }
            }
            return lastSpawned;
        }

        private (Vector2 position, int layerZ) ResolveSpawnPosition(
            RoomDefinition roomDef,
            string entityDefinitionId,
            SpawnRuleType type)
        {
            // Resolve the specific rule
            var rule = ResolveSpawnRule(roomDef, entityDefinitionId, type);

            // Calculate random position within the rule's bounds
            int x = random.Next(rule.MinX, rule.MaxX + 1);
            int y = random.Next(rule.MinY, rule.MaxY + 1);

            // Validate and get Z-level from the cache
            var cell = cacheProvider.Room.GetTopCell(roomDef.ID, x, y);

            if (cell == null)
                throw new InternalException(
                    ApplicationCode.InitializationServiceCode.NoSpawnCellFound,
                    $"Coordinate resolution failed. No valid cell architecture found at grid location ({x}, {y}) inside Room Blueprint '{roomDef.ID}'.");

            return (new Vector2(x, y), cell.Z);
        }

        private EntitySpawnRule ResolveSpawnRule(
            RoomDefinition roomDef,
            string entityDefinitionId,
            SpawnRuleType type)
        {
            // Filter rules
            var rules = roomDef.EntitySpawnRules
                .Where(r => r.EntityDefinitionID == entityDefinitionId && r.Type == type)
                .ToList();

            if (rules.Count == 0)
                throw new InternalException(
                    ApplicationCode.InitializationServiceCode.SpawnRuleMissing,
                    $"Coordinate resolution failed. Room Blueprint '{roomDef.ID}' contains no active spawning rules matching Entity Definition Type '{entityDefinitionId}' for Category '{type}'.");

            // Randomly select one applicable rule
            return rules[random.Next(rules.Count)];
        }
        #endregion
    }
}