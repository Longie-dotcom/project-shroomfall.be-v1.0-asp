using Contract.Enum.MetaDomain.Effect;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;

namespace Application.Services.AttributeService
{
    public class CombatService
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public CombatService() { }

        #region Methods
        public static float ResolveMitigatedDamage(
            EntityInstance target,
            float rawDamage,
            AttributeType damageType)
        {
            // Map incoming offense directly to your pruned defensive categories
            AttributeType resistanceType = damageType switch
            {
                AttributeType.AttackDamage => AttributeType.DamageResistance,
                AttributeType.FirePower => AttributeType.FireResistance,
                AttributeType.IcePower => AttributeType.IceResistance,
                AttributeType.EarthPower => AttributeType.EarthResistance,
                AttributeType.DarkPower => AttributeType.DarkResistance,
                AttributeType.LightPower => AttributeType.LightResistance,
                _ => AttributeType.DamageResistance
            };

            // Fetch runtime stats from target's characteristic sheet
            var characteristic = target.GetComponent<CharacteristicInstance>();
            if (characteristic == null) return 0;

            float resistanceValue = characteristic.GetCore(resistanceType);

            // Process mitigation math (Example: resistance is a percentage value where 0.25f = 25% reduction)
            float mitigationMultiplier = Math.Clamp(1.0f - resistanceValue, 0.0f, 2.0f);
            float finalDamage = rawDamage * mitigationMultiplier;

            return Math.Max(0f, finalDamage);
        }
        #endregion
    }
}