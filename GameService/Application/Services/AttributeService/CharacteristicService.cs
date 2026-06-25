using Application.Interfaces.Cache;
using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Game;
using Contract.Enum.MetaDomain.Effect;
using Domain.Definition.EntityDomain.Component;
using Domain.Definition.MetaDomain;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Shared;

namespace Application.Services.AttributeService
{
    public class CharacteristicService
    {
        #region Attributes
        private readonly IEventBus eventBus;
        private readonly ICacheProvider cacheProvider;
        #endregion

        #region Properties
        #endregion

        public CharacteristicService(
            IEventBus eventBus,
            ICacheProvider cacheProvider)
        {
            this.eventBus = eventBus;
            this.cacheProvider = cacheProvider;
        }

        #region Methods
        public void InitializeVitals(
            EntityInstance entity)
        {
            var characteristic = entity.GetComponent<CharacteristicInstance>();

            if (characteristic == null) return;
            
            foreach (var attrDef in AttributeDefinitions.AllList())
            {
                if (attrDef.DomainType != DomainType.Vital)
                    continue;

                var scaledAttr = GetScaledAttribute(characteristic, attrDef.Type);
                if (scaledAttr == null) continue;

                var (config, maxCapacity) = scaledAttr.Value;
                maxCapacity = Math.Clamp(maxCapacity, config.Min, config.Max);
                
                characteristic.SetVital(attrDef.Type, maxCapacity);
            }
        }

        public void InitializeCores(
            EntityInstance entity)
        {
            var characteristic = entity.GetComponent<CharacteristicInstance>();
            var effectContainer = entity.GetComponent<EffectContainerInstance>();

            if (characteristic == null || effectContainer == null) return;

            // Execute raw logic with no side effects
            CalculateCoresInternal(characteristic, effectContainer);
        }

        public void ApplyEffectLogic(
            EntityInstance entity,
            EffectDefinition effectDef,
            float rawDelta)
        {
            var attrDef = AttributeDefinitions.Get(effectDef.AttributeType);

            switch (attrDef.DomainType)
            {
                case DomainType.Vital:
                    ModifyVitalValue(entity, attrDef.Type, rawDelta, effectDef.SourceType ?? AttributeType.AttackDamage);
                    break;

                case DomainType.Core:
                    RecalculateCoreValues(entity);
                    break;
            }
        }

        private void ModifyVitalValue(
            EntityInstance entity,
            AttributeType type,
            float rawDelta,
            AttributeType damageFlavor)
        {
            var characteristic = entity.GetComponent<CharacteristicInstance>();
            if (characteristic == null) return;

            var transform = entity.GetComponent<TransformInstance>();
            if (transform == null) return;

            var scaledAttr = GetScaledAttribute(characteristic, type);
            if (scaledAttr == null) return;

            var attrDef = AttributeDefinitions.Get(type);
            if (attrDef.DomainType != DomainType.Vital)
                return;

            float finalDelta = rawDelta;
            if (rawDelta < 0 && type == AttributeType.Health)
            {
                finalDelta = CombatService.ResolveMitigatedDamage(entity, Math.Abs(rawDelta), damageFlavor);
                finalDelta = -finalDelta; // Re-apply the negative
            }

            var (config, dynamicCeiling) = scaledAttr.Value;
            float current = characteristic.GetVital(type);
            float next = Math.Clamp(current + finalDelta, config.Min, dynamicCeiling);

            characteristic.SetVital(type, next);

            eventBus.Publish(new EntityVitalChangedEvent(
                entityInstanceId: entity.ID,
                roomSpatialId: transform.RoomSpatialID,
                attributeType: type,
                newValue: next
            ));
        }

        private void RecalculateCoreValues(
            EntityInstance entity)
        {
            var characteristic = entity.GetComponent<CharacteristicInstance>();
            var effectContainer = entity.GetComponent<EffectContainerInstance>();

            if (characteristic == null || effectContainer == null) return;

            // Run the core mathematical calculations
            CalculateCoresInternal(characteristic, effectContainer);

            // Perform the infrastructure side-effects (Syncing / Event Bus)
            eventBus.Publish(new PlayerCharacteristicSyncEvent(
                entityInstanceId: entity.ID,
                characteristicInstance: characteristic
            ));
        }

        private void CalculateCoresInternal(
            CharacteristicInstance characteristic,
            EffectContainerInstance effectContainer)
        {
            // Group active status effects
            var activeEffectsByAttribute = new Dictionary<AttributeType, List<EffectDefinition>>();
            foreach (var activeEffect in effectContainer.ActiveEffects)
            {
                var modifier = cacheProvider.Effect.Get(activeEffect.DefinitionID);
                if (modifier == null) continue;

                if (!activeEffectsByAttribute.TryGetValue(modifier.AttributeType, out var list))
                {
                    list = new List<EffectDefinition>();
                    activeEffectsByAttribute[modifier.AttributeType] = list;
                }
                list.Add(modifier);
            }

            // Calculate and assign each Core value
            foreach (var attrDef in AttributeDefinitions.AllList())
            {
                if (attrDef.DomainType != DomainType.Core) continue;

                var scaledAttr = GetScaledAttribute(characteristic, attrDef.Type);
                if (scaledAttr == null) continue;

                float flat = 0f;
                float percent = 0f;
                float multiplier = 1f;

                if (activeEffectsByAttribute.TryGetValue(attrDef.Type, out var matchingEffects))
                {
                    foreach (var effect in matchingEffects)
                    {
                        switch (effect?.Type)
                        {
                            case EffectType.Flat: flat += effect.Value; break;
                            case EffectType.Percentage: percent += effect.Value; break;
                            case EffectType.Multiplier: multiplier *= effect.Value; break;
                        }
                    }
                }

                var (config, scaledBaseValue) = scaledAttr.Value;
                float result = (scaledBaseValue + flat) * (1f + percent) * multiplier;
                result = Math.Clamp(result, config.Min, config.Max);

                characteristic.SetCore(attrDef.Type, result);
            }
        }

        private (AttributeValue Config, float ScaledValue)? GetScaledAttribute(
            CharacteristicInstance characteristic,
            AttributeType type)
        {
            var attributePair = cacheProvider.Characteristic.GetAttributeValue(
                characteristic.DefinitionID,
                characteristic.CurrentLevel,
                type);

            if (attributePair == null) return null;

            var (attribute, growth) = attributePair.Value;
            return (attribute, attribute.BaseValue + growth.GrowthValue);
        }
        #endregion
    }
}