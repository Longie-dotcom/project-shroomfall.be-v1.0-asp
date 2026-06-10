using Application.Services.WorldService;
using Domain.Runtime.EntityDomain;

namespace Application.Systems.Resolver
{
    public readonly struct AreaEffectContext
    {
        public readonly string AreaEffectId;
        public readonly CollisionBody Body;

        public AreaEffectContext(string areaEffectId, CollisionBody body)
        {
            AreaEffectId = areaEffectId;
            Body = body;
        }
    }

    public readonly struct AreaEffectResult
    {
        public readonly string AreaEffectId;
        public readonly List<string> AffectedTargetIds;

        public AreaEffectResult(string areaEffectId, List<string> affectedTargetIds)
        {
            AreaEffectId = areaEffectId;
            AffectedTargetIds = affectedTargetIds;
        }
    }

    public class AreaEffectResolver
    {
        #region Attributes
        private readonly CollisionService collisionService;
        #endregion

        #region Properties
        #endregion

        public AreaEffectResolver(
            CollisionService collisionService)
        {
            this.collisionService = collisionService;
        }

        #region Methods
        public List<AreaEffectResult> Resolve(List<AreaEffectContext> requests)
        {
            var results = new List<AreaEffectResult>(requests.Count);

            foreach (var req in requests)
            {
                // Query overlap at its current static position
                var collision = collisionService.QueryOverlap(req.Body, req.Body.Position);
                var affectedTargetIds = new List<string>();

                foreach (var entity in collision.Entities)
                {
                    // AOEs usually affect creatures (players/monsters), ignoring walls/tiles
                    if (entity is CreatureInstance)
                    {
                        affectedTargetIds.Add(entity.ID);
                    }
                }

                results.Add(new AreaEffectResult(req.AreaEffectId, affectedTargetIds));
            }

            return results;
        }
        #endregion
    }
}