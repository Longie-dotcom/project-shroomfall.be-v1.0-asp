using Application.Services.AttributeService;
using Application.Services.EntityService;
using Application.Services.UsageService;
using Application.Services.WorldService;
using Application.Systems.Queue;

namespace Application.Systems.System
{
    public class EntityRequest
    {
        #region Attributes
        private readonly EffectService effectService;
        private readonly AIService aiService;
        private readonly MovementService movementService;
        private readonly ProjectileService projectileService;
        private readonly LifetimeService lifetimeService;
        private readonly ItemService itemService;

        private readonly ResidencyService residencyService;
        #endregion

        #region Properties
        #endregion

        public EntityRequest(
            EffectService effectService,
            AIService aiService,
            MovementService movementService,
            ProjectileService projectileService,
            LifetimeService lifetimeService,
            ItemService itemService,

            ResidencyService residencyService)
        {
            this.effectService = effectService;
            this.aiService = aiService;
            this.movementService = movementService;
            this.projectileService = projectileService;
            this.lifetimeService = lifetimeService;
            this.itemService = itemService;

            this.residencyService = residencyService;
        }

        #region Methods
        public async Task Tick(
            float dt,
            CommandBuffer commandBuffer)
        {
            aiService.Tick(dt, commandBuffer);
            effectService.Tick(dt, commandBuffer);
            movementService.Tick(dt, commandBuffer);
            projectileService.Tick(dt, commandBuffer);
            lifetimeService.Tick(dt, commandBuffer);
            itemService.Tick(dt, commandBuffer);

            await residencyService.Tick(dt);
        }
        #endregion
    }
}