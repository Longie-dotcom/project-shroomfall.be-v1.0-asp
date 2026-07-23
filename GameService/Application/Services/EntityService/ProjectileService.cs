using Application.Services.WorldService;
using Application.Services.WorldService.Creation;
using Application.Systems.Abstraction;
using Application.Systems.Queue;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;

namespace Application.Services.EntityService
{
    public class ProjectileService : ITickService
    {
        #region Attributes
        private readonly WorldContext worldContext;
        private readonly EntitySpawnService entitySpawnService;
        #endregion

        #region Properties
        #endregion

        public ProjectileService(
            WorldContext worldContext, 
            EntitySpawnService entitySpawnService)
        {
            this.worldContext = worldContext;
            this.entitySpawnService = entitySpawnService;
        }

        #region Methods
        public void Tick(
            float dt,
            CommandBuffer commandBuffer)
        {
            var entities = worldContext.GetEntities().ToList();

            foreach (var entity in entities)
            {
                var command = CreateProjectileMovementCommand(dt, entity);

                if (command != null)
                {
                    commandBuffer.Commands.Enqueue(command.Value);
                }
            }
        }

        public bool TryHandleImpact(
            EntityInstance entity)
        {
            var projectile = entity.GetComponent<ProjectileInstance>();
            if (projectile == null)
                return false;

            var transform = entity.GetComponent<TransformInstance>();
            if (transform == null)
                return false;

            if (!string.IsNullOrEmpty(projectile.OnImpactSpawnEntityDefinitionID))
                entitySpawnService.Spawn(
                    new WorldEntityCreateContext(
                        Guid.NewGuid().ToString(),
                        projectile.OnImpactSpawnEntityDefinitionID,
                        transform.RoomSpatialID,
                        transform.LayerZ,
                        transform.Position));

            return true;
        }

        private MovementCommand? CreateProjectileMovementCommand(
            float dt,
            EntityInstance entity)
        {
            var projectile = entity.GetComponent<ProjectileInstance>();
            if (projectile == null) return null;

            var transform = entity.GetComponent<TransformInstance>();
            if (transform == null) return null;

            var collision = entity.GetComponent<CollisionInstance>();
            if (collision == null) return null;

            var velocityVector = projectile.Direction * projectile.Velocity;
            var desired = transform.Position + (velocityVector * dt);

            var body = new CollisionBody(
                entity.ID,
                transform.RoomSpatialID,
                transform.Position,
                collision.CollisionOffset,
                transform.LayerZ,
                collision.CollisionShape,
                collision.Layer,
                collision.Mask);

            return new MovementCommand(entity.ID, body, desired);
        }
        #endregion
    }
}