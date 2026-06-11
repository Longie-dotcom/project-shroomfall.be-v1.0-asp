using Application.Context;
using Application.Services.WorldService;
using Application.Systems.Resolver;
using Domain.Runtime.EntityDomain;

namespace Application.Systems.Request
{
    public class ProjectileRequest
    {
        #region Attributes
        private readonly WorldContext worldContext;
        #endregion

        #region Properties
        #endregion

        public ProjectileRequest(
            WorldContext worldContext)
        {
            this.worldContext = worldContext;
        }

        #region Methods
        public (List<ProjectileContext> Contexts, List<string> Expirations) Update(float dt)
        {
            // Create fresh lists for the current frame
            var contexts = new List<ProjectileContext>();
            var expirations = new List<string>();

            // Take a snapshot to prevent "Collection was modified" exceptions
            var projectiles = worldContext.GetEntities<ProjectileInstance>().ToList();

            foreach (var proj in projectiles)
            {
                proj.TickLifetime(dt);

                if (proj.IsExpired())
                {
                    expirations.Add(proj.ID);
                    continue;
                }

                // Logic for movement/collision
                var desired = proj.Position + proj.MovementVector * proj.Velocity * dt;
                var body = new CollisionBody(
                    proj.ID,
                    proj.RoomSpatialID,
                    proj.Position,
                    proj.CollisionOffset,
                    proj.LayerZ,
                    proj.CollisionShape);

                contexts.Add(new ProjectileContext(proj.ID, body, desired));
            }

            // Return both lists as a tuple
            return (contexts, expirations);
        }
        #endregion
    }
}