using Application.Context;
using Application.Events.Event;
using Application.Interfaces.Realtime;
using Application.Services.WorldService;
using Application.Systems.Tick;
using Domain.Common;
using Domain.DomainException;
using Domain.Shared;

namespace Application.Coordinator
{
    public class SpawnCoordinator
    {
        #region Attributes
        private readonly IEventBus eventBus;
        private readonly ResidencyTick residencyTick;
        private readonly WorldContext worldContext;
        private readonly CreationService creationService;
        private readonly CollisionService collisionService;
        #endregion

        #region Properties
        #endregion

        public SpawnCoordinator(
            IEventBus eventBus,
            ResidencyTick residencyTick,
            WorldContext worldContext,
            CreationService creationService,
            CollisionService collisionService)
        {
            this.eventBus = eventBus;
            this.residencyTick = residencyTick;
            this.worldContext = worldContext;
            this.creationService = creationService;
            this.collisionService = collisionService;
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
            var creation = creationService.CreateWorldObject(
                worldObjectDefinitionId,
                roomSpatialId,
                layerZ,
                position,
                direction);

            // Validate world object's spawn point
            var worldObject = creation.WorldObject;

            var collision = collisionService.QueryPoint(
                shape: worldObject.CollisionShape,
                roomSpatialId: roomSpatialId,
                position: worldObject.Position,
                layerZ: worldObject.LayerZ);

            if (collision.IsBlocked)
                throw new BadRequest(
                    ResponseCode.SpawnCoordinator_WorldObjectCreationHasNoValidSpawn,
                    $"Position of world object: ({worldObject.Position.X}, {worldObject.Position.Y}) at room with spatail ID: {roomSpatialId} was blocked");

            // Load spawned world object on runtime
            CommitToWorld(creation);

            // Publish spawn new world object in room
            eventBus.Publish(new EntityLifecycleEvent(
                worldObject,
                worldObject.RoomSpatialID,
                EntityLifecycleType.Spawn));
        }

        private void CommitToWorld(WorldGraph context)
        {
            // Load into runtime
            worldContext.Load(context);

            // Register residency state
            foreach (var room in context.Rooms)
            {
                residencyTick.TouchRoom(room.ID);
            }
        }
        #endregion
    }
}