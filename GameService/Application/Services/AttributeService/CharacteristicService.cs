using Application.Interfaces.Cache;
using Application.Services.Abstraction.AttributeService;
using Domain.Definition.AttributeDomain.Enum;
using Domain.Document.AttributeDomain;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Shared;

namespace Application.Services.AttributeService
{
    public class CharacteristicService : ICharacteristicService
    {
        #region Attributes
        private readonly IEffectCache effectCache;
        private readonly IAttributeValueCache attributeCache;
        #endregion

        #region Properties
        #endregion

        public CharacteristicService(
        IEffectCache effectCache,
        IAttributeValueCache attributeCache)
        {
            this.effectCache = effectCache;
            this.attributeCache = attributeCache;
        }

        #region Methods
        public void InitializeVitals(
            CreatureInstance creature)
        {
            var characteristic = creature.DefinitionID;
            var level = creature.Level;

            foreach (var type in AttributeDefinitions.All().Keys)
            {
                var attrDef = AttributeDefinitions.Get(type);

                // Skip if it is not Vital value
                if (attrDef.DomainType != DomainType.Vital)
                    continue;

                var attrValue = attributeCache.Get(characteristic, type, level);

                if (attrValue == null)
                    continue;

                // Default rule
                float initialValue = attrValue.Value;

                creature.Characteristic.SetVital(type, initialValue);
            }
        }

        public float ModifyVitalValue(
            CreatureInstance creature,
            AttributeType type,
            float delta)
        {
            var characteristic = creature.Characteristic;
            var level = creature.Level;

            var attrValue = attributeCache.Get(
                characteristic.DefinitionID,
                type, 
                level);

            if (attrValue == null)
                throw new InternalException(
                    ResponseCode.CharacteristicService_MissingAttributeValue,
                    $"Missing vital attribute value for {type} at level {level}");

            var attrDef = AttributeDefinitions.Get(type);

            if (attrDef.DomainType != DomainType.Vital)
                throw new InternalException(
                    ResponseCode.CharacteristicService_InvalidNonVitalAttribute,
                    $"Attribute {type} is not a Vital type.");

            float current = characteristic.GetVital(type);
            float next = current + delta;

            float clamped = Math.Clamp(next, attrValue.Min, attrValue.Max);

            characteristic.SetVital(type, clamped);

            return clamped;
        }

        public void RehydrateVitals(
            CreatureInstance creature,
            CharacteristicDocument doc)
        {
            if (doc?.Vitals == null)
                return;

            foreach (var v in doc.Vitals)
            {
                creature.Characteristic.SetVital(v.Key, v.Value);
            }
        }

        public void RecalculateCoreValues(
            CreatureInstance creature)
        {
            var characteristic = creature.Characteristic;
            var level = creature.Level;
            var effects = creature.ActiveEffects;

            foreach (var type in AttributeDefinitions.All().Keys)
            {
                var attrValue = attributeCache.Get(characteristic.DefinitionID, type, level);

                if (attrValue == null)
                    continue;

                var attrDef = AttributeDefinitions.Get(type);

                // Skip if it is not Core value
                if (attrDef.DomainType != DomainType.Core)
                    continue;

                float baseValue = attrValue.Value;

                float flat = 0f;
                float percent = 0f;
                float multiplier = 1f;

                foreach (var effect in effects)
                {
                    var effectDef = effectCache.Get(effect.DefinitionID);

                    if (effectDef == null || effectDef.AttributeType != type)
                        continue;

                    float value = effectDef.Value;

                    switch (attrDef.Category)
                    {
                        case ValueCategory.Flat:
                            flat += value;
                            break;

                        case ValueCategory.Percentage:
                            percent += value;
                            break;

                        case ValueCategory.Multiplier:
                            multiplier *= value;
                            break;

                        case ValueCategory.Regen:
                        case ValueCategory.Flag:
                            continue;
                    }
                }

                float result = baseValue;
                result = (result + flat) * (1 + percent);
                result *= multiplier;

                result = Math.Clamp(result, attrValue.Min, attrValue.Max);

                characteristic.SetCore(type, result);
            }
        }
        #endregion
    }
}