using Application.Interfaces.Cache;
using Contract.Enum.MetaDomain.Effect;
using Domain.Definition.EntityDomain.Component;
using Domain.Definition.MetaDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.MetaDomain;

namespace Application.Services.AttributeService
{
    public class VitalService
    {
        #region Attributes
        private readonly ICacheProvider cacheProvider;
        #endregion

        public VitalService(
            ICacheProvider cacheProvider)
        {
            this.cacheProvider = cacheProvider;
        }

        #region Health
        public (VitalChangedRecord? target, VitalChangedRecord? source) ApplyHealth(
            EffectContext effectContext)
        {
            var target = effectContext.Target;
            var source = effectContext.Source;
            var effect = effectContext.Effect;

            var targetCharacteristic = target.GetComponent<CharacteristicInstance>();
            if (targetCharacteristic == null)
                return (null, null);

            var healthAttribute = GetScaledAttribute(targetCharacteristic, AttributeType.Health);
            if (healthAttribute == null)
                return (null, null);

            var (config, _, maxHealth) = healthAttribute.Value;

            VitalChangeReason reason = VitalChangeReason.Damage;

            //--------------------------------------------------------
            // Source offensive power
            //--------------------------------------------------------

            var powerType = GetPowerType(effect.AttributeType);
            float offensivePower = 0f;

            if (source != null)
            {
                var sourceCharacteristic = source.GetComponent<CharacteristicInstance>();

                if (sourceCharacteristic != null)
                {
                    offensivePower = sourceCharacteristic.GetCore(powerType);
                }
            }

            //--------------------------------------------------------
            // Scale by Effect
            //--------------------------------------------------------

            float rawDamage = ResolveRawDelta(
                offensivePower,
                effect);

            //--------------------------------------------------------
            // Resistance / Penetration
            //--------------------------------------------------------

            var resistanceType = GetResistanceType(effect.AttributeType);
            var penetrationType = GetPenetrationType(effect.AttributeType);

            float resistance = targetCharacteristic.GetCore(resistanceType);
            float penetration = 0f;

            if (source != null)
            {
                var sourceCharacteristic = source.GetComponent<CharacteristicInstance>();
                if (sourceCharacteristic != null)
                {
                    penetration = sourceCharacteristic.GetCore(penetrationType);
                }
            }

            // Penetration ignores resistance.
            float effectiveResistance =
                Math.Max(0f, resistance - penetration);

            float mitigation =
                Math.Clamp(
                    1f - effectiveResistance,
                    0f,
                    2f);

            float finalDamage =
                rawDamage * mitigation;

            //--------------------------------------------------------
            // Block
            //--------------------------------------------------------

            if (finalDamage > 0 && targetCharacteristic.GetCore(AttributeType.BlockChance) > Random.Shared.NextSingle())
            {
                reason = VitalChangeReason.Block;
                finalDamage = 0f;
            }

            //--------------------------------------------------------
            // Apply Damage
            //--------------------------------------------------------

            float previous = targetCharacteristic.GetVital(AttributeType.Health);
            float current = Math.Clamp(previous - finalDamage, config.Min, maxHealth);
            targetCharacteristic.SetVital(AttributeType.Health, current);

            //--------------------------------------------------------
            // Life Steal
            //--------------------------------------------------------
            VitalChangedRecord? sourceChanged = null;
            if (source != null && finalDamage > 0f)
            {
                var sourceCharacteristic = source.GetComponent<CharacteristicInstance>();
                if (sourceCharacteristic != null)
                {
                    float lifeSteal = sourceCharacteristic.GetCore(AttributeType.LifeSteal);
                    if (lifeSteal > 0f)
                    {
                        var sourceHealth = GetScaledAttribute(sourceCharacteristic, AttributeType.Health);
                        if (sourceHealth != null)
                        {
                            reason = VitalChangeReason.LifeSteal;
                            var (sourceConfig, _, sourceMaxHealth) = sourceHealth.Value;
                            float sourcePrevious = sourceCharacteristic.GetVital(AttributeType.Health);
                            float sourceCurrent = Math.Clamp(sourcePrevious + finalDamage * lifeSteal, sourceConfig.Min, sourceMaxHealth);
                            sourceCharacteristic.SetVital(AttributeType.Health, sourceCurrent);
                            sourceChanged = new VitalChangedRecord()
                            {
                                EntityInstanceID = source.ID,
                                Vital = AttributeType.Health,
                                PreviousValue = sourcePrevious,
                                CurrentValue = sourceCurrent,
                                Reason = reason
                            };
                        }
                    }
                }
            }

            return (new VitalChangedRecord
            {
                EntityInstanceID = target.ID,
                Vital = AttributeType.Health,
                PreviousValue = previous,
                CurrentValue = current,
                Reason = reason
            }, sourceChanged);
        }
        #endregion

        #region Energy
        public (VitalChangedRecord? target, VitalChangedRecord? source) ApplyEnergy(
            EffectContext effectContext)
        {
            var target = effectContext.Target;
            var effect = effectContext.Effect;

            VitalChangeReason reason = VitalChangeReason.EnergyConsume;

            var characteristic = target.GetComponent<CharacteristicInstance>();
            if (characteristic == null)
                return (null, null);

            var energyAttribute = GetScaledAttribute(characteristic, AttributeType.Energy);
            if (energyAttribute == null)
                return (null, null);

            var (config, _, maxEnergy) = energyAttribute.Value;
            float delta = ResolveRawDelta(0f, effect);
            float previous = characteristic.GetVital(AttributeType.Energy);
            float current = Math.Clamp(previous + delta, config.Min, maxEnergy);

            characteristic.SetVital(AttributeType.Energy, current);

            return (new VitalChangedRecord
            {
                EntityInstanceID = target.ID,
                Vital = AttributeType.Energy,
                PreviousValue = previous,
                CurrentValue = current,
                Reason = reason
            }, null);
        }
        #endregion

        #region Helpers
        private float ResolveRawDelta(
            float baseValue,
            EffectDefinition effect)
        {
            return effect.Type switch
            {
                EffectType.Flat =>
                    baseValue + effect.Value,

                EffectType.Percentage =>
                    baseValue * effect.Value,

                EffectType.Multiplier =>
                    baseValue * effect.Value,

                _ => baseValue
            };
        }

        private static AttributeType GetPowerType(AttributeType damageType)
        {
            return damageType switch
            {
                AttributeType.PhysicalDamage => AttributeType.PhysicalPower,
                AttributeType.FireDamage => AttributeType.FirePower,
                AttributeType.IceDamage => AttributeType.IcePower,
                AttributeType.EarthDamage => AttributeType.EarthPower,
                AttributeType.DarkDamage => AttributeType.DarkPower,
                AttributeType.LightDamage => AttributeType.LightPower,
                _ => AttributeType.PhysicalPower,
            };
        }

        private static AttributeType GetResistanceType(AttributeType damageType)
        {
            return damageType switch
            {
                AttributeType.PhysicalDamage => AttributeType.PhysicalResistance,
                AttributeType.FireDamage => AttributeType.FireResistance,
                AttributeType.IceDamage => AttributeType.IceResistance,
                AttributeType.EarthDamage => AttributeType.EarthResistance,
                AttributeType.DarkDamage => AttributeType.DarkResistance,
                AttributeType.LightDamage => AttributeType.LightResistance,
                _ => AttributeType.PhysicalResistance
            };
        }

        private static AttributeType GetPenetrationType(AttributeType damageType)
        {
            return damageType switch
            {
                AttributeType.PhysicalDamage => AttributeType.PhysicalPenetration,
                AttributeType.FireDamage => AttributeType.FirePenetration,
                AttributeType.IceDamage => AttributeType.IcePenetration,
                AttributeType.EarthDamage => AttributeType.EarthPenetration,
                AttributeType.DarkDamage => AttributeType.DarkPenetration,
                AttributeType.LightDamage => AttributeType.LightPenetration,
                _ => AttributeType.PhysicalPenetration
            };
        }

        private (AttributeValue Config, float BaseValue, float MaxValue)?
            GetScaledAttribute(
                CharacteristicInstance characteristic,
                AttributeType type)
        {
            var pair =
                cacheProvider.Characteristic.GetAttributeValue(
                    characteristic.DefinitionID,
                    characteristic.CurrentLevel,
                    type);

            if (pair == null)
                return null;

            var (attribute, growth) = pair.Value;

            return (
                attribute,
                attribute.BaseValue + growth.GrowthValue,
                attribute.Max + growth.GrowthValue
            );
        }
        #endregion
    }
}