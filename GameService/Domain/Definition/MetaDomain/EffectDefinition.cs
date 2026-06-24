using Contract.Enum.MetaDomain.Effect;
using Domain.Definition.LocalizationDomain;

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
        public AttributeType? SourceType { get; private set; }
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
            AttributeType? sourceType,
            float value,
            float? duration,
            float? interval,
            EffectPresentationDefinition presentation)
        {
            ID = id;
            Type = type;
            AttributeType = attributeType;
            SourceType = sourceType;
            Value = value;
            Duration = duration;
            Interval = interval;
            Presentation = presentation;
        }

        #region Methods
        public void UpdateFields(
            EffectType type,
            AttributeType attributeType,
            AttributeType? sourceType,
            float value,
            float? duration,
            float? interval)
        {
            Type = type;
            AttributeType = attributeType;
            SourceType = sourceType;
            Value = value;
            Duration = duration;
            Interval = interval;
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