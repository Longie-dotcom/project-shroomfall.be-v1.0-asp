using Application.Interfaces.Cache;
using Application.Services.WorldService;
using Application.Systems.Abstraction;
using Application.Systems.Queue;
using Domain.Definition.MetaDomain;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.MetaDomain;

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
            var effectDef = cacheProvider.Effect.Get(effectDefinitionId);

            if (container == null || effectDef == null) return;

            // INSTANT (e.g. direct damage, instant heal) - Apply and discard immediately!
            if (effectDef.Duration == 0)
            {
                characteristicService.ApplyEffectLogic(target, effectDef, effectDef.Value);
                return;
            }

            // TEMPORARY (e.g. 10s buff or 5s poison)
            if (effectDef.Duration > 0)
            {
                if (container.TemporaryEffects.TryGetValue(effectDef.ID, out var existing))
                {
                    existing.ResetTimer(effectDef.Duration.Value);
                }
                else
                {
                    container.TemporaryEffects[effectDef.ID] = new EffectInstance(effectDef.ID, effectDef.Duration, effectDef.Interval);
                    characteristicService.ApplyEffectLogic(target, effectDef, effectDef.Value);
                }
                return;
            }

            // PERMANENT (e.g. equipped armor stats, passive traits)
            if (!effectDef.Duration.HasValue)
            {
                if (!container.PermanentEffects.ContainsKey(effectDef.ID))
                {
                    container.PermanentEffects[effectDef.ID] = new EffectInstance(effectDef.ID, null, effectDef.Interval);
                    characteristicService.ApplyEffectLogic(target, effectDef, effectDef.Value);
                }
            }
        }

        public void RemoveEffect(
            EntityInstance target,
            string effectDefinitionId)
        {
            var container = target.GetComponent<EffectContainerInstance>();
            if (container == null) return;

            bool removed = container.TemporaryEffects.Remove(effectDefinitionId) ||
                           container.PermanentEffects.Remove(effectDefinitionId);

            if (removed)
            {
                var effectDef = cacheProvider.Effect.Get(effectDefinitionId);
                if (effectDef != null)
                {
                    characteristicService.ApplyEffectLogic(target, effectDef, effectDef.Value);
                }
            }
        }

        public void Tick(
            float dt, 
            CommandBuffer commandBuffer)
        {
            foreach (var entity in worldContext.GetEntities())
            {
                var container = entity.GetComponent<EffectContainerInstance>();
                if (container == null) continue;

                // --- PROCESS TEMPORARY EFFECTS ---
                if (container.TemporaryEffects.Count > 0)
                {
                    var tempKeys = container.TemporaryEffects.Keys.ToList();
                    foreach (var key in tempKeys)
                    {
                        var effect = container.TemporaryEffects[key];
                        var effectDef = cacheProvider.Effect.Get(effect.DefinitionID);
                        if (effectDef == null) continue;

                        effect.TickDuration(dt);

                        // If it's a DoT (Damage over Time) / HoT (Heal over Time), tick the interval
                        if (effect.TickInterval(dt))
                        {
                            characteristicService.ApplyEffectLogic(entity, effectDef, effectDef.Value);
                        }

                        // Handle Expiration
                        if (effect.IsExpired())
                        {
                            container.TemporaryEffects.Remove(key);
                            characteristicService.ApplyEffectLogic(entity, effectDef, effectDef.Value);
                        }
                    }
                }

                // --- PROCESS PERMANENT EFFECTS ---
                // Permanent effects don't expire, but they might have intervals (e.g. permanent passive health regen)
                if (container.PermanentEffects.Count > 0)
                {
                    foreach (var effect in container.PermanentEffects.Values)
                    {
                        if (effect.TickInterval(dt))
                        {
                            var effectDef = cacheProvider.Effect.Get(effect.DefinitionID);
                            if (effectDef != null)
                            {
                                characteristicService.ApplyEffectLogic(entity, effectDef, effectDef.Value);
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}