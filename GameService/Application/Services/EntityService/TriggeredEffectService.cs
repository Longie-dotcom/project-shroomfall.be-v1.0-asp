using Application.Interfaces.Cache;
using Application.Services.AttributeService;
using Application.Services.WorldService;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.MetaDomain;

namespace Application.Services.EntityService
{
    public class TriggeredEffectService
    {
        #region Attributes
        private readonly EffectService effectService;
        private readonly WorldContext worldContext;
        private readonly ICacheProvider cacheProvider;
        #endregion

        #region Properties
        #endregion

        public TriggeredEffectService(
            EffectService effectService,
            WorldContext worldContext,
            ICacheProvider cacheProvider)
        {
            this.effectService = effectService;
            this.worldContext = worldContext;
            this.cacheProvider = cacheProvider;
        }

        #region Methods
        public void OnEntityTouched(
            EntityInstance touched,
            EntityInstance entity)
        {
            // Scenario A:
            // Player walks into a spike trap / lava / stationary hazard.
            ApplyTriggeredEffects(
                triggeredEntity: touched,
                targetEntity: entity);

            // Scenario B:
            // Projectile flies into a player / monster.
            ApplyTriggeredEffects(
                triggeredEntity: entity,
                targetEntity: touched);
        }

        private void ApplyTriggeredEffects(
            EntityInstance triggeredEntity,
            EntityInstance targetEntity)
        {
            var triggeredEffect = triggeredEntity.GetComponent<TriggeredEffectInstance>();
            if (triggeredEffect == null)
                return;

            EntityInstance? source = null;

            if (!string.IsNullOrWhiteSpace(triggeredEffect.SourceEntityID))
            {
                source = worldContext.GetEntity(triggeredEffect.SourceEntityID);
            }

            foreach (var effectId in triggeredEffect.EffectDefinitionIDs)
            {
                var effect = cacheProvider.Effect.Get(effectId);
                if (effect == null)
                    continue;

                effectService.ApplyEffect(new EffectContext
                {
                    Source = source,
                    Target = targetEntity,
                    Effect = effect
                });
            }
        }
        #endregion
    }
}