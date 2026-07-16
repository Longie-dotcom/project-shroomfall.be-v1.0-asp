using Contract;
using Contract.Enum.MetaDomain.Effect;
using Domain.Definition.LocalizationDomain;
using Domain.DomainException;
using ResponseCode;

namespace Domain.Definition.MetaDomain
{
    public class EffectDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; } = string.Empty;
        public EffectType Type { get; private set; }
        public AttributeType AttributeType { get; private set; }
        public float Value { get; private set; }
        public float? Duration { get; private set; } // Apply for both
        public float? Interval { get; private set; } // Applied for attribute type == vital only
        public EffectPresentationDefinition Presentation { get; private set; }
        #endregion

        protected EffectDefinition() { }

        public EffectDefinition(
            string id,
            EffectType type,
            AttributeType attributeType,
            float value,
            float? duration,
            float? interval,
            EffectPresentationDefinition presentation)
        {
            Validate(ID, attributeType, duration, interval);

            ID = id;
            Type = type;
            AttributeType = attributeType;
            Value = value;
            Duration = duration;
            Interval = interval;
            Presentation = presentation;
        }

        #region Methods
        public void UpdateFields(
            EffectType type,
            AttributeType attributeType,
            float value,
            float? duration,
            float? interval)
        {
            Validate(ID, attributeType, duration, interval);

            Type = type;
            AttributeType = attributeType;
            Value = value;
            Duration = duration;
            Interval = interval;
        }

        private static void Validate(
            string id, 
            AttributeType attributeType,
            float? duration,
            float? interval)
        {
            if (attributeType == AttributeType.Health || attributeType == AttributeType.Energy)
                throw new BadRequest(
                    DomainCode.EffectDefinitionCode.DirectTargetingForbidden,
                    $"Effect definition creation failed for '{id}'. Cannot target '{attributeType}' directly. Use Restore or Damage types instead.");

            if (duration.HasValue && duration.Value < 0f)
                throw new BadRequest(
                    DomainCode.EffectDefinitionCode.DurationNegative,
                    $"Effect definition creation failed for '{id}'. Duration cannot be negative. Value: {duration}");

            if (interval.HasValue && interval.Value <= 0f)
                throw new BadRequest(
                    DomainCode.EffectDefinitionCode.IntervalInvalid,
                    $"Effect definition creation failed for '{id}'. Tick interval must be greater than 0. Value: {interval}");

            var attributeDef = AttributeDefinitions.Get(attributeType);

            if (attributeDef.DomainType == DomainType.Core && interval.HasValue)
                throw new BadRequest(
                    DomainCode.EffectDefinitionCode.CoreDomainIntervalNotSupported,
                    $"Effect definition creation failed for '{id}'. Core domain attribute '{attributeType}' does not support tick intervals.");

            if (duration.HasValue && duration.Value == 0f && interval.HasValue)
                throw new BadRequest(
                    DomainCode.EffectDefinitionCode.InstantEffectIntervalNotSupported,
                    $"Effect definition creation failed for '{id}'. An instant effect (Duration = 0) cannot have a tick interval.");
        }
        #endregion
    }

    public class EffectPresentationDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public LocalizedText LocalizedText { get; private set; } = new LocalizedText();
        public string? IconID { get; private set; } = string.Empty;
        #endregion

        protected EffectPresentationDefinition() { }

        public EffectPresentationDefinition(
            LocalizedText localizedText,
            string? iconId)
        {
            LocalizedText = localizedText;
            IconID = iconId;
        }

        #region Methods
        #endregion
    }
}