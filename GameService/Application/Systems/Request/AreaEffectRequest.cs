using Application.Context;
using Application.Services.WorldService;
using Application.Systems.Resolver;
using Domain.Runtime.EntityDomain;

namespace Application.Systems.Request
{
    public class AreaEffectRequest
    {
        #region Attributes
        private readonly WorldContext worldContext;
        #endregion

        #region Properties
        #endregion

        public AreaEffectRequest(WorldContext worldContext)
        {
            this.worldContext = worldContext;
        }

        #region Methods
        public (List<AreaEffectContext> Contexts, List<string> Expirations) Update(
            float dt)
        {
            // 1. Create fresh local lists
            var contexts = new List<AreaEffectContext>();
            var expirations = new List<string>();

            // 2. Take a snapshot to prevent "Collection was modified" exceptions
            var areaEffects = worldContext.GetEntities<AreaEffectInstance>().ToList();

            foreach (var aoe in areaEffects)
            {
                aoe.TickLifetime(dt);

                if (aoe.IsExpired())
                {
                    expirations.Add(aoe.ID);
                    continue;
                }

                // Logic for collision body
                var body = new CollisionBody(
                    aoe.ID,
                    aoe.RoomSpatialID,
                    aoe.Position,
                    aoe.CollisionOffset,
                    aoe.LayerZ,
                    aoe.CollisionShape);

                contexts.Add(new AreaEffectContext(aoe.ID, body));
            }

            // 3. Return as a Tuple
            return (contexts, expirations);
        }
        #endregion
    }
}