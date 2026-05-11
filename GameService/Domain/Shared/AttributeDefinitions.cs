using Domain.Definition.AttributeDomain;
using Domain.Definition.AttributeDomain.Enum;
using Domain.Definition.LocalizationDomain;

namespace Domain.Shared
{
    public enum DomainType
    {
        Core,        // attack, defense, movement
        Vital,       // health, stamina, energy
    }

    public enum ValueCategory
    {
        // ─────────────────────────────
        // Flat value (direct number)
        // Example: Health = 100
        // ─────────────────────────────
        Flat,

        // ─────────────────────────────
        // Percentage-based value (0–100 or 0–1 depending design)
        // Example: CritChance = +10%
        // ─────────────────────────────
        Percentage,

        // ─────────────────────────────
        // Multiplicative value
        // Example: MoveSpeed x1.2, AttackSpeed x1.5
        // ─────────────────────────────
        Multiplier,

        // ─────────────────────────────
        // Regeneration over time
        // Example: HealthRegen = +5/sec
        // ─────────────────────────────
        Regen,

        // ─────────────────────────────
        // Boolean-like or threshold-based stat
        // Example: IsImmune, CanFly (rare in your system)
        // ─────────────────────────────
        Flag
    }

    public static class AttributeDefinitions
    {
        private static readonly Dictionary<AttributeType, AttributeDefinition> map;

        static AttributeDefinitions()
        {
            map = new Dictionary<AttributeType, AttributeDefinition>
            {
                // ───────── Combat ─────────
                [AttributeType.AttackDamage] = new AttributeDefinition
                {
                    Type = AttributeType.AttackDamage,
                    Category = ValueCategory.Flat,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.attack_damage.name",
                        DescriptionKey = "parameter.attack_damage.description"
                    }
                },

                [AttributeType.AttackSpeed] = new AttributeDefinition
                {
                    Type = AttributeType.AttackSpeed,
                    Category = ValueCategory.Multiplier,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.attack_speed.name",
                        DescriptionKey = "parameter.attack_speed.description"
                    }
                },

                [AttributeType.AttackStability] = new AttributeDefinition
                {
                    Type = AttributeType.AttackStability,
                    Category = ValueCategory.Percentage,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.attack_stability.name",
                        DescriptionKey = "parameter.attack_stability.description"
                    }
                },

                [AttributeType.AttackArea] = new AttributeDefinition
                {
                    Type = AttributeType.AttackArea,
                    Category = ValueCategory.Flat,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.attack_area.name",
                        DescriptionKey = "parameter.attack_area.description"
                    }
                },

                [AttributeType.AttackRange] = new AttributeDefinition
                {
                    Type = AttributeType.AttackRange,
                    Category = ValueCategory.Flat,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.attack_range.name",
                        DescriptionKey = "parameter.attack_range.description"
                    }
                },

                [AttributeType.FirePower] = new AttributeDefinition
                {
                    Type = AttributeType.FirePower,
                    Category = ValueCategory.Multiplier,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.fire_power.name",
                        DescriptionKey = "parameter.fire_power.description"
                    }
                },

                [AttributeType.IcePower] = new AttributeDefinition
                {
                    Type = AttributeType.IcePower,
                    Category = ValueCategory.Multiplier,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.ice_power.name",
                        DescriptionKey = "parameter.ice_power.description"
                    }
                },

                [AttributeType.EarthPower] = new AttributeDefinition
                {
                    Type = AttributeType.EarthPower,
                    Category = ValueCategory.Multiplier,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.earth_power.name",
                        DescriptionKey = "parameter.earth_power.description"
                    }
                },

                [AttributeType.DarkPower] = new AttributeDefinition
                {
                    Type = AttributeType.DarkPower,
                    Category = ValueCategory.Multiplier,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.dark_power.name",
                        DescriptionKey = "parameter.dark_power.description"
                    }
                },

                [AttributeType.LightPower] = new AttributeDefinition
                {
                    Type = AttributeType.LightPower,
                    Category = ValueCategory.Multiplier,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.light_power.name",
                        DescriptionKey = "parameter.light_power.description"
                    }
                },

                // ───────── Resistance ─────────
                [AttributeType.MeleeResistance] = new AttributeDefinition
                {
                    Type = AttributeType.MeleeResistance,
                    Category = ValueCategory.Percentage,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.melee_resistance.name",
                        DescriptionKey = "parameter.melee_resistance.description"
                    }
                },

                [AttributeType.RangedResistance] = new AttributeDefinition
                {
                    Type = AttributeType.RangedResistance,
                    Category = ValueCategory.Percentage,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.ranged_resistance.name",
                        DescriptionKey = "parameter.ranged_resistance.description"
                    }
                },

                [AttributeType.MagicResistance] = new AttributeDefinition
                {
                    Type = AttributeType.MagicResistance,
                    Category = ValueCategory.Percentage,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.magic_resistance.name",
                        DescriptionKey = "parameter.magic_resistance.description"
                    }
                },

                [AttributeType.HeavyResistance] = new AttributeDefinition
                {
                    Type = AttributeType.HeavyResistance,
                    Category = ValueCategory.Percentage,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.heavy_resistance.name",
                        DescriptionKey = "parameter.heavy_resistance.description"
                    }
                },

                [AttributeType.ThrowableResistance] = new AttributeDefinition
                {
                    Type = AttributeType.ThrowableResistance,
                    Category = ValueCategory.Percentage,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.throwable_resistance.name",
                        DescriptionKey = "parameter.throwable_resistance.description"
                    }
                },

                [AttributeType.FireResistance] = new AttributeDefinition
                {
                    Type = AttributeType.FireResistance,
                    Category = ValueCategory.Percentage,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.fire_resistance.name",
                        DescriptionKey = "parameter.fire_resistance.description"
                    }
                },

                [AttributeType.IceResistance] = new AttributeDefinition
                {
                    Type = AttributeType.IceResistance,
                    Category = ValueCategory.Percentage,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.ice_resistance.name",
                        DescriptionKey = "parameter.ice_resistance.description"
                    }
                },

                [AttributeType.EarthResistance] = new AttributeDefinition
                {
                    Type = AttributeType.EarthResistance,
                    Category = ValueCategory.Percentage,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.earth_resistance.name",
                        DescriptionKey = "parameter.earth_resistance.description"
                    }
                },

                [AttributeType.DarkResistance] = new AttributeDefinition
                {
                    Type = AttributeType.DarkResistance,
                    Category = ValueCategory.Percentage,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.dark_resistance.name",
                        DescriptionKey = "parameter.dark_resistance.description"
                    }
                },

                [AttributeType.LightResistance] = new AttributeDefinition
                {
                    Type = AttributeType.LightResistance,
                    Category = ValueCategory.Percentage,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.light_resistance.name",
                        DescriptionKey = "parameter.light_resistance.description"
                    }
                },

                // ───────── Extraction ─────────
                [AttributeType.ExtractDamage] = new AttributeDefinition
                {
                    Type = AttributeType.ExtractDamage,
                    Category = ValueCategory.Flat,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.extract_damage.name",
                        DescriptionKey = "parameter.extract_damage.description"
                    }
                },

                [AttributeType.ExtractSpeed] = new AttributeDefinition
                {
                    Type = AttributeType.ExtractSpeed,
                    Category = ValueCategory.Multiplier,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.extract_speed.name",
                        DescriptionKey = "parameter.extract_speed.description"
                    }
                },

                [AttributeType.ExtractStability] = new AttributeDefinition
                {
                    Type = AttributeType.ExtractStability,
                    Category = ValueCategory.Percentage,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.extract_stability.name",
                        DescriptionKey = "parameter.extract_stability.description"
                    }
                },

                [AttributeType.ExtractArea] = new AttributeDefinition
                {
                    Type = AttributeType.ExtractArea,
                    Category = ValueCategory.Flat,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.extract_area.name",
                        DescriptionKey = "parameter.extract_area.description"
                    }
                },

                [AttributeType.ExtractRange] = new AttributeDefinition
                {
                    Type = AttributeType.ExtractRange,
                    Category = ValueCategory.Flat,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.extract_range.name",
                        DescriptionKey = "parameter.extract_range.description"
                    }
                },

                // ───────── Farming ─────────
                [AttributeType.FarmEfficiency] = new AttributeDefinition
                {
                    Type = AttributeType.FarmEfficiency,
                    Category = ValueCategory.Percentage,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.farm_efficiency.name",
                        DescriptionKey = "parameter.farm_efficiency.description"
                    }
                },

                [AttributeType.FarmQuality] = new AttributeDefinition
                {
                    Type = AttributeType.FarmQuality,
                    Category = ValueCategory.Flat,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.farm_quality.name",
                        DescriptionKey = "parameter.farm_quality.description"
                    }
                },

                // ───────── Taming ─────────
                [AttributeType.TameEfficiency] = new AttributeDefinition
                {
                    Type = AttributeType.TameEfficiency,
                    Category = ValueCategory.Percentage,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.tame_efficiency.name",
                        DescriptionKey = "parameter.tame_efficiency.description"
                    }
                },

                [AttributeType.TameQuality] = new AttributeDefinition
                {
                    Type = AttributeType.TameQuality,
                    Category = ValueCategory.Flat,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.tame_quality.name",
                        DescriptionKey = "parameter.tame_quality.description"
                    }
                },

                // ───────── Utility ─────────
                [AttributeType.MoveSpeed] = new AttributeDefinition
                {
                    Type = AttributeType.MoveSpeed,
                    Category = ValueCategory.Multiplier,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.move_speed.name",
                        DescriptionKey = "parameter.move_speed.description"
                    }
                },

                [AttributeType.Lucky] = new AttributeDefinition
                {
                    Type = AttributeType.Lucky,
                    Category = ValueCategory.Flat,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.luck.name",
                        DescriptionKey = "parameter.luck.description"
                    }
                },

                // ───────── Vital ─────────
                [AttributeType.Health] = new AttributeDefinition
                {
                    Type = AttributeType.Health,
                    Category = ValueCategory.Flat,
                    DomainType = DomainType.Vital,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.health.name",
                        DescriptionKey = "parameter.health.description"
                    }
                },

                [AttributeType.Stamina] = new AttributeDefinition
                {
                    Type = AttributeType.Stamina,
                    Category = ValueCategory.Flat,
                    DomainType = DomainType.Vital,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.stamina.name",
                        DescriptionKey = "parameter.stamina.description"
                    }
                },

                [AttributeType.Energy] = new AttributeDefinition
                {
                    Type = AttributeType.Energy,
                    Category = ValueCategory.Flat,
                    DomainType = DomainType.Vital,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.energy.name",
                        DescriptionKey = "parameter.energy.description"
                    }
                },

                [AttributeType.HealthRegen] = new AttributeDefinition
                {
                    Type = AttributeType.HealthRegen,
                    Category = ValueCategory.Flat,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.health_regen.name",
                        DescriptionKey = "parameter.health_regen.description"
                    }
                },

                [AttributeType.StaminaRegen] = new AttributeDefinition
                {
                    Type = AttributeType.StaminaRegen,
                    Category = ValueCategory.Flat,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.stamina_regen.name",
                        DescriptionKey = "parameter.stamina_regen.description"
                    }
                },

                [AttributeType.EnergyRegen] = new AttributeDefinition
                {
                    Type = AttributeType.EnergyRegen,
                    Category = ValueCategory.Flat,
                    DomainType = DomainType.Core,
                    LocalizedText = new LocalizedText
                    {
                        NameKey = "parameter.energy_regen.name",
                        DescriptionKey = "parameter.energy_regen.description"
                    }
                }
            };
        }

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
    }
}
