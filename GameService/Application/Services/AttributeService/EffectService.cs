using Application.Services.WorldService;
using Application.Systems.Abstraction;
using Application.Systems.Queue;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.MetaDomain;

namespace Application.Services.AttributeService
{
    public class EffectService : ITickService
    {
        #region Attributes
        private readonly WorldContext worldContext;
        private readonly CharacteristicService characteristicService;
        #endregion

        public EffectService(
            WorldContext worldContext,
            CharacteristicService characteristicService)
        {
            this.worldContext = worldContext;
            this.characteristicService = characteristicService;
        }

        #region Methods
        public void ApplyEffect(
            EffectContext effectContext)
        {
            var container = effectContext.Target.GetComponent<EffectContainerInstance>();
            if (container == null)
                return;

            var effect = effectContext.Effect;

            // Instant
            if (effect.Duration == 0)
            {
                characteristicService.ApplyEffectLogic(effectContext);
                return;
            }

            // Temporary
            if (effect.Duration > 0)
            {
                ApplyTemporary(container, effectContext);
                return;
            }

            // Permanent
            ApplyPermanent(container, effectContext);
        }

        public void RemoveEffect(
            EffectContext effectContext)
        {
            var effectDef = effectContext.Effect;

            var container = effectContext.Target.GetComponent<EffectContainerInstance>();
            if (container == null)
                return;

            var existing = container.TrackingEffects.FirstOrDefault(e => e.DefinitionID == effectDef.ID);
            if (existing != null)
            {
                container.TrackingEffects.Remove(existing);
                characteristicService.ApplyEffectLogic(effectContext);
            }
        }

        public void Tick(
            float dt,
            CommandBuffer commandBuffer)
        {
            foreach (var entity in worldContext.GetEntities())
            {
                var container = entity.GetComponent<EffectContainerInstance>();
                if (container == null)
                    continue;

                // Iterate on a copy since effects may expire during iteration
                foreach (var effect in container.TrackingEffects.ToList())
                {
                    effect.TickDuration(dt);

                    if (effect.TickInterval(dt))
                    {
                        // Vital effects need to be ticking
                        characteristicService.ApplyEffectLogic(effect.Context);
                    }

                    if (effect.IsExpired())
                    {
                        container.TrackingEffects.Remove(effect);

                        // Core effects need recalculation after disappearing.
                        // Vital effects with duration simply stop ticking.
                        characteristicService.ApplyEffectLogic(effect.Context);
                    }
                }
            }
        }

        private void ApplyTemporary(
            EffectContainerInstance container,
            EffectContext context)
        {
            var definition = context.Effect;

            var existing = container.TrackingEffects.FirstOrDefault(e => e.DefinitionID == definition.ID);
            if (existing != null)
            {
                existing.ResetTimer(definition.Duration!.Value);
                return;
            }

            container.TrackingEffects.Add(new EffectInstance(
                definition.ID,
                context,
                definition.Duration,
                definition.Interval));

            characteristicService.ApplyEffectLogic(context);
        }

        private void ApplyPermanent(
            EffectContainerInstance container,
            EffectContext context)
        {
            var definition = context.Effect;

            var existing = container.TrackingEffects.FirstOrDefault(e => e.DefinitionID == definition.ID);
            if (existing != null)
                return;

            container.TrackingEffects.Add(new EffectInstance(
                definition.ID,
                context,
                null,
                definition.Interval));

            characteristicService.ApplyEffectLogic(context);
        }
        #endregion
    }
}