using Application.Interfaces.Cache;
using Contract;
using Contract.Enum.MetaDomain.Effect;
using Domain.Definition.EntityDomain.Component;
using Domain.Definition.MetaDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.MetaDomain;

namespace Application.Services.AttributeService
{
    public class VitalService
    {
        private readonly ICacheProvider cacheProvider;

        public VitalService(ICacheProvider cacheProvider)
        {
            this.cacheProvider = cacheProvider;
        }

        #region Offensive / Restorative Health
        public (VitalChangedRecord? target, VitalChangedRecord? source) ApplyOffensiveHealth(
            EffectContext effectContext)
        {
            var target = effectContext.Target;
            var source = effectContext.Source;
            var effect = effectContext.Effect;

            var targetCharacteristic = target.GetComponent<CharacteristicInstance>();
            if (targetCharacteristic == null) return (null, null);

            var sourceCharacteristic = source?.GetComponent<CharacteristicInstance>();

            var healthAttribute = GetScaledAttribute(targetCharacteristic, AttributeType.Health);
            if (healthAttribute == null) return (null, null);

            var (config, _, maxHealth) = healthAttribute.Value;
            VitalChangeReason reason = VitalChangeReason.Damage;

            //--------------------------------------------------------
            // Source offensive power
            //--------------------------------------------------------
            var powerType = GetPowerType(effect.AttributeType);
            float offensivePower = sourceCharacteristic != null ? sourceCharacteristic.GetCore(powerType) : 0f;

            //--------------------------------------------------------
            // Scale by Effect
            //--------------------------------------------------------
            float rawDamage = ResolveRawDelta(offensivePower, effect);

            //--------------------------------------------------------
            // Resistance / Penetration
            //--------------------------------------------------------
            var resistanceType = GetResistanceType(effect.AttributeType);
            var penetrationType = GetPenetrationType(effect.AttributeType);

            float resistance = targetCharacteristic.GetCore(resistanceType);
            float penetration = sourceCharacteristic != null ? sourceCharacteristic.GetCore(penetrationType) : 0f;

            float effectiveResistance = Math.Max(0f, resistance - penetration);
            float mitigation = Math.Clamp(1f - effectiveResistance, 0f, 2f);
            float finalDamage = rawDamage * mitigation;

            //--------------------------------------------------------
            // Critical Chance
            //--------------------------------------------------------
            if (sourceCharacteristic != null && finalDamage > 0f)
            {
                float criticalChance = sourceCharacteristic.GetCore(AttributeType.CriticalChance);
                if (criticalChance > Random.Shared.NextSingle())
                {
                    // Override final damage to Constraint.CRITICAL_DAMAGE_VALUE of the target's current health
                    float currentTargetHealth = targetCharacteristic.GetVital(AttributeType.Health);
                    finalDamage = currentTargetHealth * Constraint.CRITICAL_DAMAGE_VALUE;
                    reason = VitalChangeReason.Critical;
                }
            }

            //--------------------------------------------------------
            // Block
            //--------------------------------------------------------
            // Note: As written, a successful block will completely negate a critical hit.
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
            if (sourceCharacteristic != null && finalDamage > 0f && source != null)
            {
                float lifeSteal = sourceCharacteristic.GetCore(AttributeType.LifeSteal);
                if (lifeSteal > 0f)
                {
                    var sourceHealth = GetScaledAttribute(sourceCharacteristic, AttributeType.Health);
                    if (sourceHealth != null)
                    {
                        var (sourceConfig, _, sourceMaxHealth) = sourceHealth.Value;
                        float sourcePrevious = sourceCharacteristic.GetVital(AttributeType.Health);
                        float sourceCurrent = Math.Clamp(sourcePrevious + finalDamage * lifeSteal, sourceConfig.Min, sourceMaxHealth);

                        sourceCharacteristic.SetVital(AttributeType.Health, sourceCurrent);

                        sourceChanged = new VitalChangedRecord
                        {
                            EntityInstanceID = source.ID,
                            Vital = AttributeType.Health,
                            PreviousValue = sourcePrevious,
                            CurrentValue = sourceCurrent,
                            Reason = VitalChangeReason.LifeSteal
                        };
                    }
                }
            }

            return (
                new VitalChangedRecord
                {
                    EntityInstanceID = target.ID,
                    Vital = AttributeType.Health,
                    PreviousValue = previous,
                    CurrentValue = current,
                    Reason = reason
                },
                sourceChanged);
        }

        public (VitalChangedRecord? target, VitalChangedRecord? source) ApplyRestorativeHealth(
            EffectContext effectContext)
        {
            var target = effectContext.Target;
            var effect = effectContext.Effect;

            var characteristic = target.GetComponent<CharacteristicInstance>();
            if (characteristic == null) return (null, null);

            var healthAttribute = GetScaledAttribute(characteristic, AttributeType.Health);
            if (healthAttribute == null) return (null, null);

            var (config, _, maxHealth) = healthAttribute.Value;

            float healAmount = ResolveRawDelta(0f, effect);
            float previous = characteristic.GetVital(AttributeType.Health);
            float current = Math.Clamp(previous + healAmount, config.Min, maxHealth);

            characteristic.SetVital(AttributeType.Health, current);

            return (
                new VitalChangedRecord
                {
                    EntityInstanceID = target.ID,
                    Vital = AttributeType.Health,
                    PreviousValue = previous,
                    CurrentValue = current,
                    Reason = VitalChangeReason.Heal
                },
                null);
        }
        #endregion

        #region Offensive / Restorative Energy
        public (VitalChangedRecord? target, VitalChangedRecord? source) ApplyOffensiveEnergy(
            EffectContext effectContext)
        {
            var target = effectContext.Target;
            var effect = effectContext.Effect;

            var characteristic = target.GetComponent<CharacteristicInstance>();
            if (characteristic == null) return (null, null);

            var energyAttribute = GetScaledAttribute(characteristic, AttributeType.Energy);
            if (energyAttribute == null) return (null, null);

            var (config, _, maxEnergy) = energyAttribute.Value;

            float consumeAmount = ResolveRawDelta(0f, effect);
            float previous = characteristic.GetVital(AttributeType.Energy);
            float current = Math.Clamp(previous - consumeAmount, config.Min, maxEnergy);

            characteristic.SetVital(AttributeType.Energy, current);

            return (
                new VitalChangedRecord
                {
                    EntityInstanceID = target.ID,
                    Vital = AttributeType.Energy,
                    PreviousValue = previous,
                    CurrentValue = current,
                    Reason = VitalChangeReason.EnergyConsume
                },
                null);
        }

        public (VitalChangedRecord? target, VitalChangedRecord? source) ApplyRestorativeEnergy(
            EffectContext effectContext)
        {
            var target = effectContext.Target;
            var effect = effectContext.Effect;

            var characteristic = target.GetComponent<CharacteristicInstance>();
            if (characteristic == null) return (null, null);

            var energyAttribute = GetScaledAttribute(characteristic, AttributeType.Energy);
            if (energyAttribute == null) return (null, null);

            var (config, _, maxEnergy) = energyAttribute.Value;

            float restoreAmount = ResolveRawDelta(0f, effect);
            float previous = characteristic.GetVital(AttributeType.Energy);
            float current = Math.Clamp(previous + restoreAmount, config.Min, maxEnergy);

            characteristic.SetVital(AttributeType.Energy, current);

            return (
                new VitalChangedRecord
                {
                    EntityInstanceID = target.ID,
                    Vital = AttributeType.Energy,
                    PreviousValue = previous,
                    CurrentValue = current,
                    Reason = VitalChangeReason.EnergyRestore 
                },
                null);
        }
        #endregion

        #region Helpers
        private float ResolveRawDelta(float baseValue, EffectDefinition effect)
        {
            return effect.Type switch
            {
                EffectType.Flat => baseValue + effect.Value,
                EffectType.Percentage => baseValue * (1 + effect.Value),
                EffectType.Multiplier => baseValue * effect.Value,
                _ => baseValue
            };
        }

        private static AttributeType GetPowerType(AttributeType damageType) => damageType switch
        {
            AttributeType.PhysicalDamage => AttributeType.PhysicalPower,
            AttributeType.FireDamage => AttributeType.FirePower,
            AttributeType.IceDamage => AttributeType.IcePower,
            AttributeType.EarthDamage => AttributeType.EarthPower,
            AttributeType.DarkDamage => AttributeType.DarkPower,
            AttributeType.LightDamage => AttributeType.LightPower,
            _ => AttributeType.PhysicalPower,
        };

        private static AttributeType GetResistanceType(AttributeType damageType) => damageType switch
        {
            AttributeType.PhysicalDamage => AttributeType.PhysicalResistance,
            AttributeType.FireDamage => AttributeType.FireResistance,
            AttributeType.IceDamage => AttributeType.IceResistance,
            AttributeType.EarthDamage => AttributeType.EarthResistance,
            AttributeType.DarkDamage => AttributeType.DarkResistance,
            AttributeType.LightDamage => AttributeType.LightResistance,
            _ => AttributeType.PhysicalResistance
        };

        private static AttributeType GetPenetrationType(AttributeType damageType) => damageType switch
        {
            AttributeType.PhysicalDamage => AttributeType.PhysicalPenetration,
            AttributeType.FireDamage => AttributeType.FirePenetration,
            AttributeType.IceDamage => AttributeType.IcePenetration,
            AttributeType.EarthDamage => AttributeType.EarthPenetration,
            AttributeType.DarkDamage => AttributeType.DarkPenetration,
            AttributeType.LightDamage => AttributeType.LightPenetration,
            _ => AttributeType.PhysicalPenetration
        };

        private (AttributeValue Config, float BaseValue, float MaxValue)? GetScaledAttribute(
            CharacteristicInstance characteristic,
            AttributeType type)
        {
            var pair = cacheProvider.Characteristic.GetAttributeValue(
                characteristic.DefinitionID,
                characteristic.CurrentLevel,
                type);

            if (pair == null) return null;

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