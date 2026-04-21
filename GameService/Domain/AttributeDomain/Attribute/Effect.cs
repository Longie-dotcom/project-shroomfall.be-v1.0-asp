using Domain.AttributeDomain.Enum;

namespace Domain.AttributeDomain.Attribute
{
    public class Effect
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; }
        public Parameter TargetParameter { get; set; }
        public EffectLifetime Lifetime { get; set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public float Value { get; private set; }
        public float? Duration { get; private set; }
        public float? Interval { get; private set; }
        #endregion

        public Effect(
            string id,
            Parameter targetParameter,
            EffectLifetime lifetime,
            string name,
            string description,
            float value,
            float? duration,
            float? interval)
        {
            ID = id;
            TargetParameter = targetParameter;
            Lifetime = lifetime;
            Name = name;
            Description = description;
            Value = value;
            Duration = duration;
            Interval = interval;
        }

        #region Methods
        #endregion
    }
}
