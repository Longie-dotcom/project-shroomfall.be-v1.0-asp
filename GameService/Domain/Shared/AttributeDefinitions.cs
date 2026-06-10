using Contract.Enum.AttributeDomain;
using Domain.Definition.AttributeDomain;

namespace Domain.Shared
{
    public static class AttributeDefinitions
    {
        private static readonly Dictionary<AttributeType, AttributeDefinition> map;

        static AttributeDefinitions()
        {
            map = new Dictionary<AttributeType, AttributeDefinition>
            {
                // ───────── Combat ─────────
                [AttributeType.AttackDamage] = CreateCore(AttributeType.AttackDamage, "attack_damage"),
                [AttributeType.AttackSpeed] = CreateCore(AttributeType.AttackSpeed, "attack_speed"),
                [AttributeType.AttackStability] = CreateCore(AttributeType.AttackStability, "attack_stability"),
                [AttributeType.AttackArea] = CreateCore(AttributeType.AttackArea, "attack_area"),
                [AttributeType.AttackRange] = CreateCore(AttributeType.AttackRange, "attack_range"),

                [AttributeType.FirePower] = CreateCore(AttributeType.FirePower, "fire_power"),
                [AttributeType.IcePower] = CreateCore(AttributeType.IcePower, "ice_power"),
                [AttributeType.EarthPower] = CreateCore(AttributeType.EarthPower, "earth_power"),
                [AttributeType.DarkPower] = CreateCore(AttributeType.DarkPower, "dark_power"),
                [AttributeType.LightPower] = CreateCore(AttributeType.LightPower, "light_power"),

                // ───────── Resistance ─────────
                [AttributeType.MeleeResistance] = CreateCore(AttributeType.MeleeResistance, "melee_resistance"),
                [AttributeType.RangedResistance] = CreateCore(AttributeType.RangedResistance, "ranged_resistance"),
                [AttributeType.MagicResistance] = CreateCore(AttributeType.MagicResistance, "magic_resistance"),
                [AttributeType.HeavyResistance] = CreateCore(AttributeType.HeavyResistance, "heavy_resistance"),
                [AttributeType.ThrowableResistance] = CreateCore(AttributeType.ThrowableResistance, "throwable_resistance"),
                [AttributeType.FireResistance] = CreateCore(AttributeType.FireResistance, "fire_resistance"),
                [AttributeType.IceResistance] = CreateCore(AttributeType.IceResistance, "ice_resistance"),
                [AttributeType.EarthResistance] = CreateCore(AttributeType.EarthResistance, "earth_resistance"),
                [AttributeType.DarkResistance] = CreateCore(AttributeType.DarkResistance, "dark_resistance"),
                [AttributeType.LightResistance] = CreateCore(AttributeType.LightResistance, "light_resistance"),

                // ───────── Extraction ─────────
                [AttributeType.ExtractDamage] = CreateCore(AttributeType.ExtractDamage, "extract_damage"),
                [AttributeType.ExtractSpeed] = CreateCore(AttributeType.ExtractSpeed, "extract_speed"),
                [AttributeType.ExtractStability] = CreateCore(AttributeType.ExtractStability, "extract_stability"),
                [AttributeType.ExtractArea] = CreateCore(AttributeType.ExtractArea, "extract_area"),
                [AttributeType.ExtractRange] = CreateCore(AttributeType.ExtractRange, "extract_range"),

                // ───────── Farming ─────────
                [AttributeType.FarmEfficiency] = CreateCore(AttributeType.FarmEfficiency, "farm_efficiency"),
                [AttributeType.FarmQuality] = CreateCore(AttributeType.FarmQuality, "farm_quality"),

                // ───────── Taming ─────────
                [AttributeType.TameEfficiency] = CreateCore(AttributeType.TameEfficiency, "tame_efficiency"),
                [AttributeType.TameQuality] = CreateCore(AttributeType.TameQuality, "tame_quality"),

                // ───────── Utility ─────────
                [AttributeType.MoveSpeed] = CreateCore(AttributeType.MoveSpeed, "move_speed"),
                [AttributeType.Lucky] = CreateCore(AttributeType.Lucky, "luck"),

                // ───────── Vital ─────────
                [AttributeType.Health] = CreateVital(AttributeType.Health, "health"),
                [AttributeType.Stamina] = CreateVital(AttributeType.Stamina, "stamina"),
                [AttributeType.Energy] = CreateVital(AttributeType.Energy, "energy"),
            };
        }

        #region Helper Factory Methods
        // Cleaned up boilerplates using small internal factory helpers
        private static AttributeDefinition CreateCore(AttributeType type, string keyName) =>
            Create(type, DomainType.Core, keyName);

        private static AttributeDefinition CreateVital(AttributeType type, string keyName) =>
            Create(type, DomainType.Vital, keyName);

        private static AttributeDefinition Create(AttributeType type, DomainType domainType, string keyName)
        {
            return new AttributeDefinition
            {
                Type = type,
                DomainType = domainType,
                LocalizedText = LocalizationFactory.ForAttribute(keyName)
            };
        }
        #endregion

        #region Queries
        public static AttributeDefinition Get(AttributeType parameter)
        {
            return map[parameter];
        }

        public static bool TryGet(AttributeType parameter, out AttributeDefinition definition)
        {
            return map.TryGetValue(parameter, out definition!);
        }

        public static IReadOnlyDictionary<AttributeType, AttributeDefinition> All()
        {
            return map;
        }

        public static IEnumerable<AttributeDefinition> AllList()
        {
            return map.Values;
        }
        #endregion
    }
}