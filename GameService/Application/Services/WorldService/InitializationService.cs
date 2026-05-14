using Application.Interfaces.Cache;
using Application.Interfaces.Factory;
using Domain.Common;
using Domain.Definition.EntityDomain;
using Domain.Definition.EntityDomain.Enum;
using Domain.Definition.WorldDomain.Enum;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Shared;

namespace Application.Services.WorldService
{
    public class InitializationService
    {
        #region Attributes
        private readonly Random random;
        private readonly IRoomCache roomCache;
        private readonly IEntityCache entityCache;
        private readonly ICreatureInstanceFactory creatureInstanceFactory;
        private readonly IWorldObjectInstanceFactory worldObjectInstanceFactory;
        private readonly SpawnService spawnService;
        #endregion

        #region Properties
        #endregion

        public InitializationService(
            IRoomCache roomCache,
            IEntityCache entityCache,
            ICreatureInstanceFactory creatureInstanceFactory,
            IWorldObjectInstanceFactory worldObjectInstanceFactory,
            SpawnService spawnService)
        {
            random = new Random();
            this.roomCache = roomCache;
            this.entityCache = entityCache;
            this.creatureInstanceFactory = creatureInstanceFactory;
            this.worldObjectInstanceFactory = worldObjectInstanceFactory;
            this.spawnService = spawnService;
        }

        #region Methods
        public WorldGraph InitializeRoomEnvironment(
            string roomSpatialId,
            string roomDefinitionId)
        {
            var worldGraph = new WorldGraph();

            // Validate room definition existence
            var roomDef = roomCache.Get(roomDefinitionId);
            if (roomDef == null)
                throw new InternalException(
                    ResponseCode.InitializationService_RoomDefinitionNotFound,
                    $"Room with definition ID: {roomDefinitionId} was not found");

            // Only initialize environment instances
            var rules = roomDef.EntitySpawnRules
                .Where(x => x.Type == SpawnRuleType.Environment)
                .ToList();
            
            // Spawn environment instances
            foreach (var rule in rules)
            {
                int totalCount = random.Next(
                    rule.SpawnAreas.Min(x => x.MinCount),
                    rule.SpawnAreas.Max(x => x.MaxCount) + 1);

                for (int i = 0; i < totalCount; i++)
                {
                    var area = spawnService.PickWeightedArea(
                        rule.SpawnAreas);

                    var (position, layerZ) =
                        spawnService.ResolveSpawnPosition(
                            roomDefinitionId,
                            area);

                    var entityDef = entityCache.Get<Entity>(rule.EntityID);

                    if (entityDef == null)
                        continue;

                    switch (entityDef.Type)
                    {
                        case EntityType.WorldObject:
                            {
                                var worldObject =
                                    SpawnWorldObject(
                                        worldObjectDefinitionId: rule.EntityID,
                                        roomSpatialId: roomSpatialId,
                                        layerZ: layerZ,
                                        position: position,
                                        direction: new Vector2(0, 1),
                                        worldGraph);

                                worldGraph.Entities.Add(worldObject);
                                break;
                            }

                        case EntityType.Creature:
                            {
                                var creature =
                                    SpawnCreature(
                                        creatureDefinitionId: rule.EntityID,
                                        roomSpatialId: roomSpatialId,
                                        layerZ: layerZ,
                                        position: position,
                                        direction: new Vector2(0, 1));

                                worldGraph.Entities.Add(creature);
                                break;
                            }
                    }
                }
            }

            return worldGraph;
        }

        private WorldObjectInstance SpawnWorldObject(
            string worldObjectDefinitionId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 direction,
            WorldGraph worldGraph)
        {
            var timestamp = Guid.NewGuid().ToString("N");
            var worldObjectInstanceId = $"WORLD_OBJECT_{timestamp}";

            var (worldObject, linkedRoomSpatialId, linkedRoomDefinitionId) =
                worldObjectInstanceFactory.Create(
                    definitionId: worldObjectDefinitionId,
                    instanceId: worldObjectInstanceId,
                    roomSpatialId: roomSpatialId,
                    layerZ: layerZ,
                    position: position,
                    direction: direction);

            if (!string.IsNullOrWhiteSpace(linkedRoomSpatialId)
                && !string.IsNullOrWhiteSpace(linkedRoomDefinitionId))
            {
                worldGraph.PendingRooms.Add(new PendingRoomInitialization
                {
                    RoomSpatialID = linkedRoomSpatialId,
                    RoomDefinitionID = linkedRoomDefinitionId
                });
            }

            return worldObject;
        }

        private CreatureInstance SpawnCreature(
            string creatureDefinitionId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 direction)
        {
            var timestamp = Guid.NewGuid().ToString("N");
            var creatureInstanceId = $"CREATURE_{timestamp}";

            var creature = creatureInstanceFactory.Create(
                definitionId: creatureDefinitionId,
                instanceId: creatureInstanceId,
                roomSpatialId: roomSpatialId,
                layerZ: layerZ,
                position: position,
                direction: direction
            );

            return creature;
        }
        #endregion
    }
}