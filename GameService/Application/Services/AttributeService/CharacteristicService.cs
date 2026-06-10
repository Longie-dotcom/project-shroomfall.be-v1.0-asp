using Application.Events.Event;
using Application.Interfaces.Cache;
using Application.Interfaces.Realtime;
using AutoMapper;
using Contract.DTO.Runtime;
using Contract.Enum.AttributeDomain;
using Domain.Document.AttributeDomain;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Shared;

namespace Application.Services.AttributeService
{
    public class CharacteristicService
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly IEventBus eventBus;
        private readonly IEffectCache effectCache;
        private readonly IAttributeValueCache attributeCache;
        #endregion

        #region Properties
        #endregion

        public CharacteristicService(
            IMapper mapper,
            IEventBus eventBus,
            IEffectCache effectCache,
            IAttributeValueCache attributeCache)
        {
            this.mapper = mapper;
            this.eventBus = eventBus;
            this.effectCache = effectCache;
            this.attributeCache = attributeCache;
        }

        #region Methods
        public void InitializeVitals(
            CreatureInstance creature)
        {
            var characteristicId = creature.DefinitionID;
            var level = creature.Level;

            foreach (var attrDef in AttributeDefinitions.AllList())
            {
                // Skip if it is not a Vital value
                if (attrDef.DomainType != DomainType.Vital)
                    continue;

                var attrValue = attributeCache.Get(characteristicId, attrDef.Type, level);
                if (attrValue == null)
                    continue;

                creature.Characteristic.SetVital(attrDef.Type, attrValue.Value);
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

            eventBus.Publish(new EntityVitalChangedEvent(
                entityInstanceId: creature.ID,
                roomSpatialId: creature.RoomSpatialID,
                attributeType: type,
                newValue: clamped,
                occurredAt: DateTime.UtcNow
            ));

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

            var activeEffectsByAttribute = creature.ActiveEffects
                .Select(e => effectCache.Get(e.DefinitionID))
                .Where(def => def != null)
                .GroupBy(def => def.AttributeType)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var attrDef in AttributeDefinitions.AllList())
            {
                // Skip if it is not Core value
                if (attrDef.DomainType != DomainType.Core)
                    continue;

                var attrValue = attributeCache.Get(characteristic.DefinitionID, attrDef.Type, level);
                if (attrValue == null)
                    continue;

                float baseValue = attrValue.Value;
                float flat = 0f;
                float percent = 0f;
                float multiplier = 1f;

                if (activeEffectsByAttribute.TryGetValue(attrDef.Type, out var matchingEffects))
                {
                    foreach (var effect in matchingEffects)
                    {
                        switch (effect?.Type)
                        {
                            case EffectType.Flat:
                                flat += effect.Value;
                                break;

                            case EffectType.Percentage:
                                percent += effect.Value;
                                break;

                            case EffectType.Multiplier:
                                multiplier *= effect.Value;
                                break;
                        }
                    }
                }

                // 4. Run the decoupled math processing algorithm
                float result = (baseValue + flat) * (1f + percent) * multiplier;
                result = Math.Clamp(result, attrValue.Min, attrValue.Max);

                characteristic.SetCore(attrDef.Type, result);
            }

            var dto = mapper.Map<CharacteristicRuntimeDTO>(characteristic);

            eventBus.Publish(new PlayerCharacteristicSyncEvent(
                entityInstanceId: creature.ID,
                characteristicRuntime: dto,
                occurredAt: DateTime.UtcNow
            ));
        }
        #endregion
    }
}