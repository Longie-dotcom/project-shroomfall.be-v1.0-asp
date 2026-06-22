using Application.Context;
using Application.Systems.Abstraction;
using Application.Systems.Queue;
using Domain.Runtime.EntityDomain.Component;

namespace Application.Services.EntityService
{
    public class LifetimeService : ITickService
    {
        #region Attributes
        private readonly WorldContext worldContext;
        #endregion

        #region Properties
        #endregion

        public LifetimeService(
            WorldContext worldContext)
        {
            this.worldContext = worldContext;
        }

        #region Methods
        public void Tick(
            float dt,
            CommandBuffer commandBuffer)
        {
            var entities = worldContext.GetEntities().ToList();

            foreach (var entity in entities)
            {
                var lifetime = entity.GetComponent<LifetimeInstance>();
                if (lifetime == null)
                    continue;

                lifetime.TickLifetime(dt);

                if (lifetime.IsExpired())
                {
                    commandBuffer.Commands.Enqueue(new EntityExpiredCommand(entity.ID));
                }
            }
        }
        #endregion
    }
}