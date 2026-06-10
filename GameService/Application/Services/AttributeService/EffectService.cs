using Application.Interfaces.Cache;
using Application.Interfaces.Factory;
using Contract.Enum.AttributeDomain;
using Domain.Definition.ItemDomain;
using Domain.Runtime.EntityDomain;
using Domain.Shared;

namespace Application.Services.AttributeService
{
    public class EffectService
    {
        #region Attributes
        private readonly IEffectInstanceFactory effectFactory;
        private readonly CharacteristicService characteristicService;
        private readonly IItemCache itemCache;
        private readonly IEffectCache effectCache;
        #endregion

        #region Properties
        #endregion

        public EffectService(
            IEffectInstanceFactory effectFactory,
            CharacteristicService characteristicService,
            IItemCache itemCache,
            IEffectCache effectCache)
        {
            this.effectFactory = effectFactory;
            this.characteristicService = characteristicService;
            this.itemCache = itemCache;
            this.effectCache = effectCache;
        }

        #region Methods
        // Active
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

        // Passive 
        public void ExecuteInstantPayload(
            CreatureInstance target,
            string sourceDefinitionId,
            string sourceInstanceId)
        {
            var definition = itemCache.Get(sourceDefinitionId);
            if (definition == null) return;

            foreach (var effectRef in definition.Effects)
            {
                var effectDef = effectCache.Get(effectRef.EffectID);
                if (effectDef == null) continue;
                
                var attrDef = AttributeDefinitions.Get(effectDef.AttributeType);
                if (attrDef == null) continue;

                if (effectDef.Duration.HasValue)
                {
                    var liveEffect = effectFactory.Create(effectRef.EffectID, sourceInstanceId);
                    target.ActiveEffects.Add(liveEffect);

                    // If it targets a Core value, recalculate stats immediately so the buff/debuff applies right away
                    if (attrDef.DomainType == DomainType.Core)
                    {
                        characteristicService.RecalculateCoreValues(target);
                    }
                    continue;
                }

                // Check if this effect targets a Vital pool (like Health or Mana)
                if (attrDef.DomainType == DomainType.Vital)
                {
                    // If the database says -50, it reduces health. If it says +25, it restores health.
                    characteristicService.ModifyVitalValue(
                        target,
                        effectDef.AttributeType,
                        effectDef.Value);
                }
            }
        }
        #endregion
    }
}