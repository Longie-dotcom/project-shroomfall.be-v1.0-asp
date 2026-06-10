using Application.Services.WorldService;
using Domain.Common;

namespace Application.Systems.Resolver
{
    public readonly struct ProjectileContext
    {
        public readonly string ProjectileId;
        public readonly CollisionBody Body;
        public readonly Vector2 DesiredPosition;

        public ProjectileContext(string projectileId, CollisionBody body, Vector2 desiredPosition)
        {
            ProjectileId = projectileId;
            Body = body;
            DesiredPosition = desiredPosition;
        }
    }

    public readonly struct ProjectileResult
    {
        public Vector2 FinalPosition { get; init; }
        public bool DidImpact { get; init; }
        public List<string> HitTargetIds { get; init; } 
    }

    public class ProjectileResolver
    {
        #region Attributes
        private readonly CollisionService collisionService;
        #endregion

        #region Properties
        #endregion

        public ProjectileResolver(
            CollisionService collisionService)
        {
            this.collisionService = collisionService;
        }

        #region Methods
        public Dictionary<string, ProjectileResult> Resolve(
            List<ProjectileContext> requests)
        {
            var results = new Dictionary<string, ProjectileResult>(requests.Count);

            foreach (var req in requests)
            {
                var collision = collisionService.QueryOverlap(req.Body, req.DesiredPosition);
                var hitTargetIds = new List<string>();

                // Gather all blocking entities inside our collision box this frame
                foreach (var entity in collision.Entities)
                {
                    if (entity.CollisionShape.IsBlocking)
                    {
                        hitTargetIds.Add(entity.ID);
                    }
                }

                results[req.ProjectileId] = new ProjectileResult
                {
                    // If it hit *anything* blocking (tile or entity), it impacted
                    FinalPosition = collision.IsBlocked ? req.Body.Position : req.DesiredPosition,
                    DidImpact = collision.IsBlocked,
                    HitTargetIds = hitTargetIds
                };
            }

            return results;
        }
        #endregion
    }
}