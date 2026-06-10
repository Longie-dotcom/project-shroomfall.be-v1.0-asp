using Contract.Enum.AttributeDomain;
using Domain.Definition.AttributeDomain;
using Domain.Shared;

namespace Infrastructure.Persistence.Seeder
{
    public static class EffectSeeder
    {
        #region 🏷️ True Special Status IDs (Exposed for Sprite / UI Drawing)
        // These are the exact IDs saved in the Database. 
        // Your UI/Sprite system can look these up directly to draw status icons!
        public const string EFFECT_BURN_ID = "status_effect_burn";
        public const string EFFECT_SLOW_ID = "status_effect_slow";
        public const string EFFECT_WEIGHT_SLOW_ID = "status_effect_weight_slow";
        #endregion

        #region 🆔 Standard Vital / Attribute Suffix Helpers
        public static string HpId(string baseId) => $"{baseId}_hp";
        public static string StaminaId(string baseId) => $"{baseId}_stamina";
        public static string EnergyId(string baseId) => $"{baseId}_energy";

        public static string MoveSpeedPermId(string baseId) => $"{baseId}_movespeed_perm";
        public static string LuckPermId(string baseId) => $"{baseId}_luck_perm";
        public static string FireResPermId(string baseId) => $"{baseId}_fire_res_perm";
        public static string IceResPermId(string baseId) => $"{baseId}_ice_res_perm";
        public static string EarthResPermId(string baseId) => $"{baseId}_earth_res_perm";
        #endregion

        public static async Task SeedEffectDefinitionsAsync(RelationalDB db)
        {
            var effects = new List<Effect>();

            // ═══════════════════════════════════════════════════════════════════
            // 🧪 CONSUMABLES
            // ═══════════════════════════════════════════════════════════════════
            effects.AddRange(CreateConsumableInstantVitalEffects(ItemSeeder.HP_POTION_ID, healthRestore: 50f, staminaRestore: 0f, energyRestore: 0f));
            effects.AddRange(CreateConsumableInstantVitalEffects(ItemSeeder.STAMINA_POTION_ID, healthRestore: 0f, staminaRestore: 60f, energyRestore: 0f));

            // ═══════════════════════════════════════════════════════════════════
            // ⚔️ WEAPONS & COMBAT
            // ═══════════════════════════════════════════════════════════════════

            // MELEE
            effects.AddRange(CreateWeaponInstantVitalDamageEffects(ItemSeeder.IRON_SWORD_ID, healthDamage: 18f, staminaDamage: 5f, energyDamage: 0f));
            effects.AddRange(CreateWeaponInstantVitalDamageEffects(ItemSeeder.WOODEN_CLUB_ID, healthDamage: 12f, staminaDamage: 25f, energyDamage: 0f));

            // RANGE
            effects.AddRange(CreateWeaponInstantVitalDamageEffects(ItemSeeder.WOODEN_BOW_ID, healthDamage: 14f, staminaDamage: 2f, energyDamage: 0f));
            effects.AddRange(CreateWeaponInstantVitalDamageEffects(ItemSeeder.CROSSBOW_ID, healthDamage: 26f, staminaDamage: 12f, energyDamage: 0f));

            // THROWABLES (Directly using our standalone static IDs)
            effects.AddRange(CreateWeaponInstantVitalDamageEffects(ItemSeeder.FIRE_BOMB_ID, healthDamage: 15f, staminaDamage: 0f, energyDamage: 0f));
            effects.Add(new Effect(EFFECT_BURN_ID, EffectType.Flat, LocalizationFactory.ForEffect(EFFECT_BURN_ID), AttributeType.Health, value: -8f, duration: 5f, interval: 1f));

            effects.AddRange(CreateWeaponInstantVitalDamageEffects(ItemSeeder.STUN_GRENADE_ID, healthDamage: 0f, staminaDamage: 50f, energyDamage: 0f));
            effects.Add(new Effect(EFFECT_SLOW_ID, EffectType.Percentage, LocalizationFactory.ForEffect(EFFECT_SLOW_ID), AttributeType.MoveSpeed, value: -0.60f, duration: 3f, interval: null));

            // ═══════════════════════════════════════════════════════════════════
            // 🛡️ EQUIPMENT
            // ═══════════════════════════════════════════════════════════════════
            effects.AddRange(CreateEquipmentPermanentCoreEffects(ItemSeeder.IRON_HELMET_ID, EffectType.Flat, moveSpeedBuff: 0f, luckyBuff: 0f, fireResBuff: 5f, iceResBuff: 2f, earthResBuff: 2f));

            // Iron Chest: Base stats + static heavy weight penalty ID
            effects.AddRange(CreateEquipmentPermanentCoreEffects(ItemSeeder.IRON_CHEST_ID, EffectType.Flat, moveSpeedBuff: 0f, luckyBuff: 0f, fireResBuff: 15f, iceResBuff: 10f, earthResBuff: 10f));
            effects.Add(new Effect(EFFECT_WEIGHT_SLOW_ID, EffectType.Percentage, LocalizationFactory.ForEffect(EFFECT_WEIGHT_SLOW_ID), AttributeType.MoveSpeed, value: -0.05f, duration: null, interval: null));

            effects.AddRange(CreateEquipmentPermanentCoreEffects(ItemSeeder.IRON_PANTS_ID, EffectType.Flat, moveSpeedBuff: 0f, luckyBuff: 0f, fireResBuff: 8f, iceResBuff: 5f, earthResBuff: 5f));
            effects.AddRange(CreateEquipmentPermanentCoreEffects(ItemSeeder.IRON_SHOES_ID, EffectType.Flat, moveSpeedBuff: 0f, luckyBuff: 0f, fireResBuff: 4f, iceResBuff: 4f, earthResBuff: 4f));

            await db.Set<Effect>().AddRangeAsync(effects);
            await db.SaveChangesAsync();
        }

        #region 🛠️ Factory Helpers (Kept for clean vital list spawning)
        private static List<Effect> CreateWeaponInstantVitalDamageEffects(string baseId, float healthDamage, float staminaDamage, float energyDamage)
        {
            var effects = new List<Effect>();
            if (healthDamage > 0)
                effects.Add(new Effect(HpId(baseId), EffectType.Flat, LocalizationFactory.ForEffect(HpId(baseId)), AttributeType.Health, -healthDamage, null, null));
            if (staminaDamage > 0)
                effects.Add(new Effect(StaminaId(baseId), EffectType.Flat, LocalizationFactory.ForEffect(StaminaId(baseId)), AttributeType.Stamina, -staminaDamage, null, null));
            if (energyDamage > 0)
                effects.Add(new Effect(EnergyId(baseId), EffectType.Flat, LocalizationFactory.ForEffect(EnergyId(baseId)), AttributeType.Energy, -energyDamage, null, null));
            return effects;
        }

        private static List<Effect> CreateConsumableInstantVitalEffects(string baseId, float healthRestore, float staminaRestore, float energyRestore)
        {
            var effects = new List<Effect>();
            if (healthRestore > 0)
                effects.Add(new Effect(HpId(baseId), EffectType.Flat, LocalizationFactory.ForEffect(HpId(baseId)), AttributeType.Health, healthRestore, null, null));
            if (staminaRestore > 0)
                effects.Add(new Effect(StaminaId(baseId), EffectType.Flat, LocalizationFactory.ForEffect(StaminaId(baseId)), AttributeType.Stamina, staminaRestore, null, null));
            if (energyRestore > 0)
                effects.Add(new Effect(EnergyId(baseId), EffectType.Flat, LocalizationFactory.ForEffect(EnergyId(baseId)), AttributeType.Energy, energyRestore, null, null));
            return effects;
        }

        private static List<Effect> CreateEquipmentPermanentCoreEffects(string baseId, EffectType type, float moveSpeedBuff, float luckyBuff, float fireResBuff, float iceResBuff, float earthResBuff)
        {
            var effects = new List<Effect>();
            if (moveSpeedBuff > 0)
                effects.Add(new Effect(MoveSpeedPermId(baseId), type, LocalizationFactory.ForEffect(MoveSpeedPermId(baseId)), AttributeType.MoveSpeed, moveSpeedBuff, null, null));
            if (luckyBuff > 0)
                effects.Add(new Effect(LuckPermId(baseId), type, LocalizationFactory.ForEffect(LuckPermId(baseId)), AttributeType.Lucky, luckyBuff, null, null));
            if (fireResBuff > 0)
                effects.Add(new Effect(FireResPermId(baseId), type, LocalizationFactory.ForEffect(FireResPermId(baseId)), AttributeType.FireResistance, fireResBuff, null, null));
            if (iceResBuff > 0)
                effects.Add(new Effect(IceResPermId(baseId), type, LocalizationFactory.ForEffect(IceResPermId(baseId)), AttributeType.IceResistance, iceResBuff, null, null));
            if (earthResBuff > 0)
                effects.Add(new Effect(EarthResPermId(baseId), type, LocalizationFactory.ForEffect(EarthResPermId(baseId)), AttributeType.EarthResistance, earthResBuff, null, null));
            return effects;
        }
        #endregion
    }
}