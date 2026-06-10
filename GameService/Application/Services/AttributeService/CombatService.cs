using Contract.Enum.AttributeDomain;
using Domain.Runtime.EntityDomain;

namespace Application.Services.AttributeService
{
    public class CombatService
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public CombatService()
        {

        }

        #region Methods
        public static float ResolveMitigatedDamage(
            CreatureInstance attacker,
            CreatureInstance target)
        {
            // 1. Define the pairs of Power vs Resistance
            var combatPairs = new[]
            {
                (AttributeType.FirePower,    AttributeType.FireResistance),
                (AttributeType.IcePower,     AttributeType.IceResistance),
                (AttributeType.EarthPower,   AttributeType.EarthResistance),
                (AttributeType.DarkPower,    AttributeType.DarkResistance),
                (AttributeType.LightPower,   AttributeType.LightResistance)
            };

            // 2. Start with base Physical AttackDamage
            float totalRawDamage = attacker.Characteristic.GetCore(AttributeType.AttackDamage);

            // 3. Sum all elemental powers and apply their specific resistance mitigations
            foreach (var (powerType, resType) in combatPairs)
            {
                float power = attacker.Characteristic.GetCore(powerType);
                if (power <= 0) continue;

                float resistance = target.Characteristic.GetCore(resType);

                // Calculate mitigation for this specific element
                float multiplier = Math.Clamp(1.0f - resistance, 0.0f, 1.0f);

                totalRawDamage += (power * multiplier);
            }

            // 4. Final physical mitigation (Optional: apply overall DamageResistance to the physical base)
            float physRes = target.Characteristic.GetCore(AttributeType.DamageResistance);
            totalRawDamage *= Math.Clamp(1.0f - physRes, 0.0f, 1.0f);

            return Math.Max(0f, totalRawDamage);
        }

        public static float ResolveMitigatedDamage(
            CreatureInstance target,
            float rawDamage,
            AttributeType damageType)
        {
            // 1. Map incoming offense directly to your pruned defensive categories
            AttributeType resistanceType = damageType switch
            {
                AttributeType.AttackDamage => AttributeType.DamageResistance,
                AttributeType.FirePower => AttributeType.FireResistance,
                AttributeType.IcePower => AttributeType.IceResistance,
                AttributeType.EarthPower => AttributeType.EarthResistance,
                AttributeType.DarkPower => AttributeType.DarkResistance,
                AttributeType.LightPower => AttributeType.LightResistance,

                // Fallback catch-all
                _ => AttributeType.DamageResistance
            };

            // 2. Fetch runtime stats from target's characteristic sheet
            float resistanceValue = target.Characteristic.GetCore(resistanceType);

            // 3. Process mitigation math (Example: resistance is a percentage value where 0.25f = 25% reduction)
            float mitigationMultiplier = Math.Clamp(1.0f - resistanceValue, 0.0f, 2.0f);
            float finalDamage = rawDamage * mitigationMultiplier;

            // Prevent negative calculations from accidentally performing healing cycles
            return Math.Max(0f, finalDamage);
        }
        #endregion
    }
}