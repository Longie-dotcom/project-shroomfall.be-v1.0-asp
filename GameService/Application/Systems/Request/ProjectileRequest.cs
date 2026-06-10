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
        public void Update(
            float dt,
            List<ProjectileContext> contexts,
            List<string> immediateExpirations)
        {
            foreach (var proj in worldContext.GetEntities<ProjectileInstance>())
            {
                // Single-pass internal ticking
                proj.TickLifetime(dt);
                if (proj.IsExpired())
                {
                    immediateExpirations.Add(proj.ID);
                    continue;
                }

                var desired = proj.Position + proj.MovementVector * proj.Velocity * dt;
                var body = new CollisionBody(proj.ID, proj.RoomSpatialID, proj.Position, proj.LayerZ, proj.CollisionShape);

                contexts.Add(new ProjectileContext(proj.ID, body, desired));
            }
        }
        #endregion
    }
}