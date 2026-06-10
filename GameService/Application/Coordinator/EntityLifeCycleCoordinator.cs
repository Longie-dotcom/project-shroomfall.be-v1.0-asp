using Application.Context;
using Application.Events.Event;
using Application.Interfaces.Factory;
using Application.Interfaces.Realtime;
using Application.Services.WorldService;
using Domain.Common;
using Domain.Runtime.AttributeDomain;
using Domain.Runtime.EntityDomain;

namespace Application.Coordinator
{
    public class EntityLifeCycleCoordinator
    {
        #region Attributes
        private readonly IEventBus eventBus;
        private readonly WorldContext worldContext;
        private readonly CollisionService collisionService;
        private readonly IWorldObjectInstanceFactory worldObjectInstanceFactory;
        private readonly IProjectileInstanceFactory projectileInstanceFactory;
        private readonly IAreaEffectInstanceFactory areaEffectInstanceFactory;
        #endregion

        #region Properties
        #endregion

        public EntityLifeCycleCoordinator(
            IEventBus eventBus,
            WorldContext worldContext,
            CollisionService collisionService,
            IWorldObjectInstanceFactory worldObjectInstanceFactory,
            IProjectileInstanceFactory projectileInstanceFactory,
            IAreaEffectInstanceFactory areaEffectInstanceFactory)
        {
            this.eventBus = eventBus;
            this.worldContext = worldContext;
            this.collisionService = collisionService;
            this.worldObjectInstanceFactory = worldObjectInstanceFactory;
            this.areaEffectInstanceFactory = areaEffectInstanceFactory;
            this.projectileInstanceFactory = projectileInstanceFactory;
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

        public void SpawnProjectile(
            string projectileDefinitionId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 direction,
            string ownerId,
            string sourceDefinitionId)
        {
            var instanceId = $"PROJECTILE_{Guid.NewGuid():N}";

            var projectile = projectileInstanceFactory.Create(
                definitionId: projectileDefinitionId,
                instanceId: instanceId,
                roomSpatialId: roomSpatialId,
                layerZ: layerZ,
                position: position,
                movementVector: direction,
                entityInstanceOwnerId: ownerId,
                sourceDefinitionId: sourceDefinitionId);

            collisionService.ValidateSpawn(projectile.CollisionShape, roomSpatialId, projectile.Position, layerZ);
            worldContext.AddEntity(projectile);

            eventBus.Publish(new EntityLifecycleEvent(
                projectile, 
                roomSpatialId,
                EntityLifecycleType.Spawn));
        }

        public void SpawnAreaEffect(
            string areaEffectDefinitionId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            string ownerId,
            string sourceDefinitionId)
        {
            var instanceId = $"AREA_EFFECT_{Guid.NewGuid():N}";

            var areaEffect = areaEffectInstanceFactory.Create(
                definitionId: areaEffectDefinitionId,
                instanceId: instanceId,
                roomSpatialId: roomSpatialId,
                layerZ: layerZ,
                position: position,
                movementVector: Vector2.Zero,
                entityInstanceOwnerId: ownerId,
                sourceDefinitionId: sourceDefinitionId);

            worldContext.AddEntity(areaEffect);
            eventBus.Publish(new EntityLifecycleEvent(
                areaEffect, 
                roomSpatialId, 
                EntityLifecycleType.Spawn));
        }

        public void Despawn(
            EntityInstance entity)
        {
            if (entity == null) return;

            // Wipe it from the runtime state & spatial maps
            worldContext.RemoveEntity(entity.ID);

            // Broadcast the removal so network clients drop the rendering instance
            eventBus.Publish(new EntityLifecycleEvent(
                entity,
                entity.RoomSpatialID,
                EntityLifecycleType.Despawn));
        }
        #endregion
    }
}