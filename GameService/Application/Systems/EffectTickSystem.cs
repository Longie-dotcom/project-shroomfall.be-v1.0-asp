using Application.Services.Abstraction.AttributeService;
using Domain.Abstraction.World;
using Domain.Runtime.EntityDomain;

namespace Application.Systems
{
    public class EffectTickSystem
    {
        #region Attributes
        private readonly IWorldQuery world;
        private readonly ICharacteristicService characteristicService;
        private readonly IEffectService effectService;
        #endregion

        #region Properties
        #endregion

        public EffectTickSystem(
            IWorldQuery world,
            ICharacteristicService characteristicService,
            IEffectService effectService)
        {
            this.world = world;
            this.characteristicService = characteristicService;
            this.effectService = effectService;
        }

        #region Methods
        public void Update(float dt)
        {
            foreach (var creature in world.GetAll<CreatureInstance>())
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