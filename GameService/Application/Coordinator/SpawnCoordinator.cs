using Application.Context;
using Application.Events.Event;
using Application.Interfaces.Factory;
using Application.Interfaces.Realtime;
using Application.Services.WorldService;
using Domain.Common;

namespace Application.Coordinator
{
    public class SpawnCoordinator
    {
        #region Attributes
        private readonly IEventBus eventBus;
        private readonly WorldContext worldContext;
        private readonly CollisionService collisionService;
        private readonly IWorldObjectInstanceFactory worldObjectInstanceFactory;
        #endregion

        #region Properties
        #endregion

        public SpawnCoordinator(
            IEventBus eventBus,
            WorldContext worldContext,
            CollisionService collisionService,
            IWorldObjectInstanceFactory worldObjectInstanceFactory)
        {
            this.eventBus = eventBus;
            this.worldContext = worldContext;
            this.collisionService = collisionService;
            this.worldObjectInstanceFactory = worldObjectInstanceFactory;
        }

        #region Methods
        public void SpawnWorldObject(
            string worldObjectDefinitionId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 direction)
        {
            // Create new world object instance and expand linking rooms
            var worldObjectInstanceId = $"WORLD_OBJECT_{Guid.NewGuid():N}";

            var worldObject = worldObjectInstanceFactory.Create(
                definitionId: worldObjectDefinitionId,
                instanceId: worldObjectInstanceId,
                roomSpatialId: roomSpatialId,
                layerZ: layerZ,
                position: position,
                direction: direction);

            // Validate world object's spawn point
            collisionService.ValidateSpawn(
                shape: worldObject.CollisionShape,
                roomSpatialId: roomSpatialId,
                position: worldObject.Position,
                layerZ: worldObject.LayerZ);

            // Load spawned world object on runtime
            worldContext.AddEntity(worldObject);

            // Publish spawn new world object in room
            eventBus.Publish(new EntityLifecycleEvent(
                worldObject,
                worldObject.RoomSpatialID,
                EntityLifecycleType.Spawn));
        }
        #endregion
    }
}