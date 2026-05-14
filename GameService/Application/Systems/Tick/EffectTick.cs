using Application.Context;
using Application.Services.AttributeService;
using Domain.Runtime.EntityDomain;

namespace Application.Systems.Tick
{
    public class EffectTick
    {
        #region Attributes
        private readonly WorldContext worldContext;
        private readonly CharacteristicService characteristicService;
        #endregion

        #region Properties
        #endregion

        public EffectTick(
            WorldContext worldContext,
            CharacteristicService characteristicService)
        {
            this.worldContext = worldContext;
            this.characteristicService = characteristicService;
        }

        #region Methods
        public void Tick(
            float dt)
        {
            foreach (var creature in worldContext.GetEntities<CreatureInstance>())
            {
                bool changed = false;

                for (int i = creature.ActiveEffects.Count - 1; i >= 0; i--)
                {
                    var effect = creature.ActiveEffects[i];

                    // Only temporal effects should tick
                    if (effect.IsPermanent())
                        continue;

                    effect.Tick(dt);

                    if (effect.IsExpired())
                    {
                        creature.ActiveEffects.RemoveAt(i);
                        changed = true;
                    }
                }

                if (changed)
                {
                    characteristicService.RecalculateCoreValues(creature);
                }
            }
        }
        #endregion
    }
}