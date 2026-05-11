using Application.Interfaces.Factory;
using Application.Services.Abstraction.AttributeService;
using Domain.Definition.ItemDomain;
using Domain.Runtime.EntityDomain;

namespace Application.Services.AttributeService
{
    public class EffectService : IEffectService
    {
        #region Attributes
        private readonly IEffectInstanceFactory effectFactory;
        private readonly ICharacteristicService characteristicService;
        #endregion

        #region Properties
        #endregion

        public EffectService(
            IEffectInstanceFactory effectFactory,
            ICharacteristicService characteristicService)
        {
            this.effectFactory = effectFactory;
            this.characteristicService = characteristicService;
        }

        #region Methods
        public void ApplyItemEffects(
            CreatureInstance creature,
            Item itemDef,
            string sourceItemInstanceId)
        {
            foreach (var itemEffect in itemDef.Effects)
            {
                var effect = effectFactory.Create(
                    itemEffect.EffectID,
                    sourceItemInstanceId);

                creature.ActiveEffects.Add(effect);
            }

            characteristicService.RecalculateCoreValues(creature);
        }

        public void RemoveItemEffects(
            CreatureInstance creature,
            string sourceItemInstanceId)
        {
            creature.ActiveEffects.RemoveAll(
                x => x.SourceItemInstanceID == sourceItemInstanceId);

            characteristicService.RecalculateCoreValues(creature);
        }
        #endregion
    }
}