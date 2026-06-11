using Contract.Enum.AttributeDomain;
using Domain.Definition.AttributeDomain;
using Domain.Shared;

namespace Infrastructure.Persistence.Seeder
{
    public static class CharacteristicSeeder
    {
        public static async Task SeedAsync(RelationalDB db)
        {
            var characteristics = new List<Characteristic>
    {
        // ─────────────────────────────────────────────────────────
        // ⚔️ WARRIOR PLAYER TEMPLATE
        // ─────────────────────────────────────────────────────────
        CreateCreatureSheet(EntitySeeder.WarriorCharacteristicId, "player_warrior", AddWarriorStats),

        // ─────────────────────────────────────────────────────────
        // 🏹 ARCHER PLAYER TEMPLATE
        // ─────────────────────────────────────────────────────────
        CreateCreatureSheet(EntitySeeder.ArcherCharacteristicId, "player_archer", AddArcherStats),

        // ─────────────────────────────────────────────────────────
        // 🃏 JOKER PLAYER TEMPLATE
        // ─────────────────────────────────────────────────────────
        CreateCreatureSheet(EntitySeeder.JokerCharacteristicId, "player_joker", AddJokerStats),

        // ─────────────────────────────────────────────────────────
        // 🔥 FIRE SHROOM
        // ─────────────────────────────────────────────────────────
        CreateCreatureSheet(
            EntitySeeder.FireShroomCharacteristicId,
            "creature_shroom_fire",
            AddFireShroomStats),

        // ─────────────────────────────────────────────────────────
        // ❄️ ICE SHROOM
        // ─────────────────────────────────────────────────────────
        CreateCreatureSheet(
            EntitySeeder.IceShroomCharacteristicId,
            "creature_shroom_ice",
            AddIceShroomStats),

        // ─────────────────────────────────────────────────────────
        // ⛰️ EARTH SHROOM
        // ─────────────────────────────────────────────────────────
        CreateCreatureSheet(
            EntitySeeder.EarthShroomCharacteristicId,
            "creature_shroom_earth",
            AddEarthShroomStats),

        // ─────────────────────────────────────────────────────────
        // 🌌 DARK SHROOM
        // ─────────────────────────────────────────────────────────
        CreateCreatureSheet(
            EntitySeeder.DarkShroomCharacteristicId,
            "creature_shroom_dark",
            AddDarkShroomStats),

        // ─────────────────────────────────────────────────────────
        // ☀️ LIGHT SHROOM
        // ─────────────────────────────────────────────────────────
            CreateCreatureSheet(
                EntitySeeder.LightShroomCharacteristicId,
                "creature_shroom_light",
                AddLightShroomStats)
        };

            await db.Set<Characteristic>().AddRangeAsync(characteristics);
            await db.SaveChangesAsync();
        }

        private static Characteristic CreateCreatureSheet(string id, string localizationKey, Action<Characteristic> statBuilder)
        {
            var characteristic = new Characteristic(
                id: id,
                type: CharacteristicType.Creature,
                localizedText: LocalizationFactory.ForEntity(localizationKey)
            );

            statBuilder(characteristic);
            return characteristic;
        }

        #region ⚔️ Warrior Stat Sheet Configuration
        private static void AddWarriorStats(Characteristic c)
        {
            // ⚔️ Combat
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackDamage, 16f, 1, 0f, 999f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackSpeed, 1.0f, 1, 0.1f, 5.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackRange, 1.8f, 1, 0.5f, 50.0f, c.ID));

            c.AttributeValues.Add(new AttributeValue(AttributeType.FirePower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.IcePower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.EarthPower, 1.25f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.DarkPower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.LightPower, 1.0f, 1, 0f, 10.0f, c.ID));

            // 🛡️ Resistance
            c.AttributeValues.Add(new AttributeValue(AttributeType.DamageResistance, 0.25f, 1, 0f, 0.90f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.FireResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.IceResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.EarthResistance, 10f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.DarkResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.LightResistance, 0f, 1, 0f, 100f, c.ID));

            // 🪓 Extraction
            c.AttributeValues.Add(new AttributeValue(AttributeType.ExtractDamage, 12f, 1, 0f, 500f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.ExtractSpeed, 0.9f, 1, 0.1f, 5.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.ExtractRange, 1.5f, 1, 0.5f, 10.0f, c.ID));

            // 🌾 Farming
            c.AttributeValues.Add(new AttributeValue(AttributeType.FarmEfficiency, 1.0f, 1, 0.1f, 5.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.FarmQuality, 0f, 1, 0f, 1.0f, c.ID));

            // 🐾 Taming
            c.AttributeValues.Add(new AttributeValue(AttributeType.TameEfficiency, 1.0f, 1, 0.1f, 5.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.TameQuality, 0f, 1, 0f, 1.0f, c.ID));

            // 🏃 Utility
            c.AttributeValues.Add(new AttributeValue(AttributeType.MoveSpeed, 9.0f, 1, 0.2f, 25.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.Lucky, 0f, 1, 0f, 100f, c.ID));

            // ❤️ Vitals
            c.AttributeValues.Add(new AttributeValue(AttributeType.Health, 120f, 1, 10f, 5000f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.Stamina, 90f, 1, 10f, 1000f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.Energy, 60f, 1, 0f, 1000f, c.ID));
        }
        #endregion

        #region 🏹 Archer Stat Sheet Configuration
        private static void AddArcherStats(Characteristic c)
        {
            // ⚔️ Combat
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackDamage, 11f, 1, 0f, 999f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackSpeed, 1.35f, 1, 0.1f, 5.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackRange, 8.5f, 1, 0.5f, 50.0f, c.ID));

            c.AttributeValues.Add(new AttributeValue(AttributeType.FirePower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.IcePower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.EarthPower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.DarkPower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.LightPower, 1.25f, 1, 0f, 10.0f, c.ID));

            // 🛡️ Resistance
            c.AttributeValues.Add(new AttributeValue(AttributeType.DamageResistance, 0.25f, 1, 0f, 0.90f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.FireResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.IceResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.EarthResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.DarkResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.LightResistance, 10f, 1, 0f, 100f, c.ID));

            // 🪓 Extraction
            c.AttributeValues.Add(new AttributeValue(AttributeType.ExtractDamage, 9f, 1, 0f, 500f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.ExtractSpeed, 1.1f, 1, 0.1f, 5.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.ExtractRange, 1.5f, 1, 0.5f, 10.0f, c.ID));

            // 🌾 Farming
            c.AttributeValues.Add(new AttributeValue(AttributeType.FarmEfficiency, 1.0f, 1, 0.1f, 5.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.FarmQuality, 0f, 1, 0f, 1.0f, c.ID));

            // 🐾 Taming
            c.AttributeValues.Add(new AttributeValue(AttributeType.TameEfficiency, 1.1f, 1, 0.1f, 5.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.TameQuality, 0f, 1, 0f, 1.0f, c.ID));

            // 🏃 Utility
            c.AttributeValues.Add(new AttributeValue(AttributeType.MoveSpeed, 9.0f, 1, 0.2f, 25.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.Lucky, 5f, 1, 0f, 100f, c.ID));

            // ❤️ Vitals
            c.AttributeValues.Add(new AttributeValue(AttributeType.Health, 85f, 1, 10f, 5000f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.Stamina, 120f, 1, 10f, 1000f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.Energy, 80f, 1, 0f, 1000f, c.ID));
        }
        #endregion

        #region 🃏 Joker Stat Sheet Configuration
        private static void AddJokerStats(Characteristic c)
        {
            // ⚔️ Combat
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackDamage, 8f, 1, 0f, 999f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackSpeed, 1.15f, 1, 0.1f, 5.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackRange, 4.5f, 1, 0.5f, 50.0f, c.ID));

            c.AttributeValues.Add(new AttributeValue(AttributeType.FirePower, 1.40f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.IcePower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.EarthPower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.DarkPower, 1.15f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.LightPower, 1.0f, 1, 0f, 10.0f, c.ID));

            // 🛡️ Resistance
            c.AttributeValues.Add(new AttributeValue(AttributeType.DamageResistance, 0.25f, 1, 0f, 0.90f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.FireResistance, 20f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.IceResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.EarthResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.DarkResistance, 5f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.LightResistance, 0f, 1, 0f, 100f, c.ID));

            // 🪓 Extraction
            c.AttributeValues.Add(new AttributeValue(AttributeType.ExtractDamage, 10f, 1, 0f, 500f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.ExtractSpeed, 1.0f, 1, 0.1f, 5.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.ExtractRange, 1.5f, 1, 0.5f, 10.0f, c.ID));

            // 🌾 Farming
            c.AttributeValues.Add(new AttributeValue(AttributeType.FarmEfficiency, 1.0f, 1, 0.1f, 5.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.FarmQuality, 0f, 1, 0f, 1.0f, c.ID));

            // 🐾 Taming
            c.AttributeValues.Add(new AttributeValue(AttributeType.TameEfficiency, 1.0f, 1, 0.1f, 5.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.TameQuality, 0f, 1, 0f, 1.0f, c.ID));

            // 🏃 Utility
            c.AttributeValues.Add(new AttributeValue(AttributeType.MoveSpeed, 9.0f, 1, 0.2f, 25.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.Lucky, 12f, 1, 0f, 100f, c.ID));

            // ❤️ Vitals
            c.AttributeValues.Add(new AttributeValue(AttributeType.Health, 100f, 1, 10f, 5000f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.Stamina, 110f, 1, 10f, 1000f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.Energy, 110f, 1, 0f, 1000f, c.ID));
        }

        private static void AddFireShroomStats(Characteristic c)
        {
            // ⚔️ Combat
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackDamage, 8f, 1, 0f, 999f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackSpeed, 1.15f, 1, 0.1f, 5.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackRange, 4.5f, 1, 0.5f, 50.0f, c.ID));

            c.AttributeValues.Add(new AttributeValue(AttributeType.FirePower, 1.50f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.IcePower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.EarthPower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.DarkPower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.LightPower, 1.0f, 1, 0f, 10.0f, c.ID));

            // 🛡️ Resistance
            c.AttributeValues.Add(new AttributeValue(AttributeType.DamageResistance, 0.25f, 1, 0f, 0.90f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.FireResistance, 25f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.IceResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.EarthResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.DarkResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.LightResistance, 0f, 1, 0f, 100f, c.ID));

            c.AttributeValues.Add(new AttributeValue(AttributeType.MoveSpeed, 9.0f, 1, 0.2f, 25.0f, c.ID));

            // ❤️ Vitals
            c.AttributeValues.Add(new AttributeValue(AttributeType.Health, 100f, 1, 10f, 5000f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.Stamina, 110f, 1, 10f, 1000f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.Energy, 110f, 1, 0f, 1000f, c.ID));
        }

        private static void AddIceShroomStats(Characteristic c)
        {
            // ⚔️ Combat
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackDamage, 8f, 1, 0f, 999f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackSpeed, 1.15f, 1, 0.1f, 5.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackRange, 4.5f, 1, 0.5f, 50.0f, c.ID));

            c.AttributeValues.Add(new AttributeValue(AttributeType.FirePower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.IcePower, 1.50f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.EarthPower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.DarkPower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.LightPower, 1.0f, 1, 0f, 10.0f, c.ID));

            // 🛡️ Resistance
            c.AttributeValues.Add(new AttributeValue(AttributeType.DamageResistance, 0.25f, 1, 0f, 0.90f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.FireResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.IceResistance, 25f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.EarthResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.DarkResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.LightResistance, 0f, 1, 0f, 100f, c.ID));

            c.AttributeValues.Add(new AttributeValue(AttributeType.MoveSpeed, 9.0f, 1, 0.2f, 25.0f, c.ID));

            // ❤️ Vitals
            c.AttributeValues.Add(new AttributeValue(AttributeType.Health, 100f, 1, 10f, 5000f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.Stamina, 110f, 1, 10f, 1000f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.Energy, 110f, 1, 0f, 1000f, c.ID));
        }

        private static void AddEarthShroomStats(Characteristic c)
        {
            // ⚔️ Combat
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackDamage, 8f, 1, 0f, 999f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackSpeed, 1.15f, 1, 0.1f, 5.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackRange, 4.5f, 1, 0.5f, 50.0f, c.ID));

            c.AttributeValues.Add(new AttributeValue(AttributeType.FirePower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.IcePower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.EarthPower, 1.50f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.DarkPower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.LightPower, 1.0f, 1, 0f, 10.0f, c.ID));

            // 🛡️ Resistance
            c.AttributeValues.Add(new AttributeValue(AttributeType.DamageResistance, 0.25f, 1, 0f, 0.90f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.FireResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.IceResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.EarthResistance, 25f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.DarkResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.LightResistance, 0f, 1, 0f, 100f, c.ID));

            c.AttributeValues.Add(new AttributeValue(AttributeType.MoveSpeed, 9.0f, 1, 0.2f, 25.0f, c.ID));

            // ❤️ Vitals
            c.AttributeValues.Add(new AttributeValue(AttributeType.Health, 100f, 1, 10f, 5000f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.Stamina, 110f, 1, 10f, 1000f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.Energy, 110f, 1, 0f, 1000f, c.ID));
        }

        private static void AddDarkShroomStats(Characteristic c)
        {
            // ⚔️ Combat
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackDamage, 8f, 1, 0f, 999f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackSpeed, 1.15f, 1, 0.1f, 5.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackRange, 4.5f, 1, 0.5f, 50.0f, c.ID));

            c.AttributeValues.Add(new AttributeValue(AttributeType.FirePower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.IcePower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.EarthPower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.DarkPower, 1.50f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.LightPower, 1.0f, 1, 0f, 10.0f, c.ID));

            // 🛡️ Resistance
            c.AttributeValues.Add(new AttributeValue(AttributeType.DamageResistance, 0.25f, 1, 0f, 0.90f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.FireResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.IceResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.EarthResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.DarkResistance, 25f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.LightResistance, 0f, 1, 0f, 100f, c.ID));

            c.AttributeValues.Add(new AttributeValue(AttributeType.MoveSpeed, 9.0f, 1, 0.2f, 25.0f, c.ID));

            // ❤️ Vitals
            c.AttributeValues.Add(new AttributeValue(AttributeType.Health, 100f, 1, 10f, 5000f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.Stamina, 110f, 1, 10f, 1000f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.Energy, 110f, 1, 0f, 1000f, c.ID));
        }

        private static void AddLightShroomStats(Characteristic c)
        {
            // ⚔️ Combat
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackDamage, 8f, 1, 0f, 999f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackSpeed, 1.15f, 1, 0.1f, 5.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.AttackRange, 4.5f, 1, 0.5f, 50.0f, c.ID));

            c.AttributeValues.Add(new AttributeValue(AttributeType.FirePower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.IcePower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.EarthPower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.DarkPower, 1.0f, 1, 0f, 10.0f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.LightPower, 1.50f, 1, 0f, 10.0f, c.ID));

            // 🛡️ Resistance
            c.AttributeValues.Add(new AttributeValue(AttributeType.DamageResistance, 0.25f, 1, 0f, 0.90f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.FireResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.IceResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.EarthResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.DarkResistance, 0f, 1, 0f, 100f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.LightResistance, 25f, 1, 0f, 100f, c.ID));

            c.AttributeValues.Add(new AttributeValue(AttributeType.MoveSpeed, 9.0f, 1, 0.2f, 25.0f, c.ID));

            // ❤️ Vitals
            c.AttributeValues.Add(new AttributeValue(AttributeType.Health, 100f, 1, 10f, 5000f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.Stamina, 110f, 1, 10f, 1000f, c.ID));
            c.AttributeValues.Add(new AttributeValue(AttributeType.Energy, 110f, 1, 0f, 1000f, c.ID));
        }
        #endregion
    }
}