namespace Domain.AttributeDomain.Enum
{
    public enum Parameter
    {
        // ───────── Combat ─────────
        AttackDamage,
        AttackSpeed,
        AttackStability,
        AttackArea,
        AttackRange,

        // ───────── Resistance ─────────
        MeleeResistance,
        RangedResistance,
        MagicResistance,
        HeavyResistance,
        ThrowableResistance,

        // ───────── Tool / Extraction ─────────
        ExtractDamage,
        ExtractSpeed,
        ExtractStability,
        ExtractArea,
        ExtractRange,

        // ───────── Farming / Taming ─────────
        FarmEfficiency,
        FarmQuality,
        TameEfficiency,
        TameQuality,

        // ───────── Movement / Utility ─────────
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
