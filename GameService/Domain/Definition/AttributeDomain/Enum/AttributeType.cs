namespace Domain.Definition.AttributeDomain.Enum
{
    public enum AttributeType
    {
        // ───────── Combat ─────────
        AttackDamage,
        AttackSpeed,
        AttackStability,
        AttackArea,
        AttackRange,

        FirePower,
        IcePower,
        EarthPower,
        DarkPower,
        LightPower,

        // ───────── Resistance ─────────
        MeleeResistance,
        RangedResistance,
        MagicResistance,
        HeavyResistance,
        ThrowableResistance,

        FireResistance,
        IceResistance,
        EarthResistance,
        DarkResistance,
        LightResistance,

        // ───────── Extraction ─────────
        ExtractDamage,
        ExtractSpeed,
        ExtractStability,
        ExtractArea,
        ExtractRange,

        // ───────── Farming ─────────
        FarmEfficiency,
        FarmQuality,

        // ───────── Taming ─────────
        TameEfficiency,
        TameQuality,

        // ───────── Utility ─────────
        MoveSpeed,
        Lucky,

        // ───────── Vital System ─────────
        Health,
        Stamina,
        Energy,
        HealthRegen,
        StaminaRegen,
        EnergyRegen
    }
}
