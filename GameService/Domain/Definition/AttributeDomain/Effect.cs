using Contract.Enum.AttributeDomain;
using Domain.Definition.LocalizationDomain;
using Domain.DomainException;
using Domain.Shared;

namespace Domain.Definition.AttributeDomain
{
    public class Effect
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; }
        public EffectType Type { get; private set; }
        public LocalizedText LocalizedText { get; private set; }
        public AttributeType AttributeType { get; set; }
        public float Value { get; private set; }
        public float? Duration { get; private set; }
        public float? Interval { get; private set; }
        #endregion

        protected Effect()
        {

        }

        public Effect(
            string id,
            EffectType type,
            LocalizedText localizedText,
            AttributeType attributeType,
            float value,
            float? duration,
            float? interval)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadRequest(ResponseCode.Effect_InvalidId);

            if (string.IsNullOrWhiteSpace(localizedText.NameKey))
                throw new BadRequest(ResponseCode.Effect_InvalidName);

            if (string.IsNullOrWhiteSpace(localizedText.DescriptionKey))
                throw new BadRequest(ResponseCode.Effect_InvalidDescription);

            if (duration.HasValue && duration.Value < 0)
                throw new BadRequest(ResponseCode.Effect_InvalidDuration);

            if (interval.HasValue && interval.Value < 0)
                throw new BadRequest(ResponseCode.Effect_InvalidInterval);

            ID = id;
            Type = type;
            LocalizedText = localizedText;
            AttributeType = attributeType;
            Value = value;
            Duration = duration;
            Interval = interval;
        }

        #region Methods
        #endregion
    }
}
