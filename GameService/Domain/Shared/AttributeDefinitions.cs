using Contract.Enum.MetaDomain.Effect;
using Domain.Definition.LocalizationDomain;

namespace Domain.Shared
{
    public class AttributeDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public AttributeType Type { get; set; }
        public LocalizedText LocalizedText { get; set; } = new LocalizedText();
        public DomainType DomainType { get; set; }
        #endregion

        #region Methods
        #endregion
    }

    public static class AttributeDefinitions
    {
        private static readonly Dictionary<AttributeType, AttributeDefinition> map;

        static AttributeDefinitions()
        {
            map = new Dictionary<AttributeType, AttributeDefinition>
            {
                // ───────── Combat ─────────
                [AttributeType.AttackDamage] = CreateCore(AttributeType.AttackDamage, "attack_damage"),
                [AttributeType.FirePower] = CreateCore(AttributeType.FirePower, "fire_power"),
                [AttributeType.IcePower] = CreateCore(AttributeType.IcePower, "ice_power"),
                [AttributeType.EarthPower] = CreateCore(AttributeType.EarthPower, "earth_power"),
                [AttributeType.DarkPower] = CreateCore(AttributeType.DarkPower, "dark_power"),
                [AttributeType.LightPower] = CreateCore(AttributeType.LightPower, "light_power"),

                // ───────── Resistance ─────────
                [AttributeType.DamageResistance] = CreateCore(AttributeType.DamageResistance, "damage_resistance"),
                [AttributeType.FireResistance] = CreateCore(AttributeType.FireResistance, "fire_resistance"),
                [AttributeType.IceResistance] = CreateCore(AttributeType.IceResistance, "ice_resistance"),
                [AttributeType.EarthResistance] = CreateCore(AttributeType.EarthResistance, "earth_resistance"),
                [AttributeType.DarkResistance] = CreateCore(AttributeType.DarkResistance, "dark_resistance"),
                [AttributeType.LightResistance] = CreateCore(AttributeType.LightResistance, "light_resistance"),

                // ───────── Utility ─────────
                [AttributeType.MoveSpeed] = CreateCore(AttributeType.MoveSpeed, "move_speed"),
                [AttributeType.Lucky] = CreateCore(AttributeType.Lucky, "luck"),
                [AttributeType.AttackSpeed] = CreateCore(AttributeType.AttackSpeed, "attack_speed"),
                [AttributeType.AttackRange] = CreateCore(AttributeType.AttackRange, "attack_range"),

                // ───────── Vital ─────────
                [AttributeType.Health] = CreateVital(AttributeType.Health, "health"),
                [AttributeType.Stamina] = CreateVital(AttributeType.Stamina, "stamina"),
                [AttributeType.Energy] = CreateVital(AttributeType.Energy, "energy"),
            };
        }

        #region Helper Factory Methods
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