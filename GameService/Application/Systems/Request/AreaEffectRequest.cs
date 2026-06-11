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
        public void Update(
            float dt,
            List<AreaEffectContext> contexts,
            List<string> immediateExpirations)
        {
            foreach (var aoe in worldContext.GetEntities<AreaEffectInstance>())
            {
                aoe.TickLifetime(dt);
                if (aoe.IsExpired())
                {
                    immediateExpirations.Add(aoe.ID);
                    continue;
                }

                // Area effects query their current static location, not a future swept location
                var body = new CollisionBody(aoe.ID, aoe.RoomSpatialID, aoe.Position, aoe.CollisionOffset, aoe.LayerZ, aoe.CollisionShape);
                contexts.Add(new AreaEffectContext(aoe.ID, body));
            }
        }
        #endregion
    }
}