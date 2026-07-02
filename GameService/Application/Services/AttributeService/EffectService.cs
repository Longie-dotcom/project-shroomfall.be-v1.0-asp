using Application.Interfaces.Cache;
using Application.Services.WorldService;
using Application.Systems.Abstraction;
using Application.Systems.Queue;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Shared;

namespace Application.Services.AttributeService
{
    public class EffectService : ITickService
    {
        #region Attributes
        private readonly WorldContext worldContext;
        private readonly ICacheProvider cacheProvider;
        private readonly CharacteristicService characteristicService;
        #endregion

        public EffectService(
            WorldContext worldContext,
            ICacheProvider cacheProvider,
            CharacteristicService characteristicService)
        {
            this.worldContext = worldContext;
            this.cacheProvider = cacheProvider;
            this.characteristicService = characteristicService;
        }

        #region Methods
        public void ApplyEffect(
            EntityInstance target, 
            string effectDefinitionId)
        {
            var container = target.GetComponent<EffectContainerInstance>();
            if (container == null) return;

            var effectDef = cacheProvider.Effect.Get(effectDefinitionId);
            if (effectDef == null) return;

            // Apply stack rule
            var existing = container.ActiveEffects.FirstOrDefault(e => e.DefinitionID == effectDef.ID);
            if (existing != null && effectDef.Duration.HasValue)
            {
                existing.ResetTimer(effectDef.Duration.Value);
                return;
            }

            var effectInstance = new EffectInstance(effectDef.ID, effectDef.Duration, effectDef.Interval);
            container.ActiveEffects.Add(effectInstance);
        }

        public void RemoveEffect(
            EntityInstance target,
            string effectDefinitionId)
        {
            var container = target.GetComponent<EffectContainerInstance>();
            if (container == null) return;

            var effectInstance = container.ActiveEffects.FirstOrDefault(e => e.DefinitionID == effectDefinitionId);
            if (effectInstance != null)
            {
                container.ActiveEffects.Remove(effectInstance);
            }
        }

        public void Tick(
            float dt,
            CommandBuffer commandBuffer)
        {
            var entities = worldContext.GetEntities().ToList();

            foreach (var entity in entities)
            {
                var container = entity.GetComponent<EffectContainerInstance>();
                if (container == null || !container.ActiveEffects.Any()) continue;

                for (int i = container.ActiveEffects.Count - 1; i >= 0; i--)
                {
                    var effect = container.ActiveEffects[i];

                    var effectDef = cacheProvider.Effect.Get(effect.DefinitionID);
                    if (effectDef == null) continue;

                    var attrDef = AttributeDefinitions.Get(effectDef.AttributeType);
                    if (attrDef == null) continue;

                    // 1. INITIAL TRIGGER (For new effects)
                    if (!effect.HasProcessedInitial)
                    {
                        characteristicService.ApplyEffectLogic(entity, effectDef, effectDef.Value);

                        effect.MarkProcessed();

                        if (effect.IsInstant())
                        {
                            container.ActiveEffects.RemoveAt(i);
                            continue;
                        }
                        continue;
                    }

                    // 2. PERIODIC TRIGGER (Existing effects only)
                    if (effect.Tick(dt))
                    {
                        characteristicService.ApplyEffectLogic(entity, effectDef, effectDef.Value);
                    }

                    // 3. EXPIRATION
                    if (effect.IsExpired())
                    {
                        container.ActiveEffects.RemoveAt(i);
                        characteristicService.ApplyEffectLogic(entity, effectDef, effectDef.Value);
                    }
                }
            }
        }
        #endregion
    }
}