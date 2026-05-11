using Domain.Definition.AttributeDomain.Enum;
using Domain.Shared;

namespace Application.DTO.Definition
{
    public class AttributeDefinitionDTO
    {
        public AttributeType Type { get; set; }
        public LocalizedTextDTO LocalizedText { get; set; } = new LocalizedTextDTO();
        public ValueCategory Category { get; set; }
        public DomainType DomainType { get; set; }
    }

    public class AttributeValueDefinitionDTO
    {
        public AttributeType Type { get; set; }
        public float Value { get; set; }
        public int Level { get; set; }
        public float Min { get; set; }
        public float Max { get; set; }
        public string CharacteristicID { get; set; } = string.Empty;
    }

    public class CharacteristicDefinitionDTO
    {
        public string ID { get; set; } = string.Empty;
        public CharacteristicType Type { get; set; }
        public LocalizedTextDTO LocalizedText { get; set; } = new LocalizedTextDTO();
    }

    public class EffectDefinitionDTO
    {
        public string ID { get; set; } = string.Empty;
        public EffectType Type { get; set; }
        public LocalizedTextDTO LocalizedText { get; set; } = new LocalizedTextDTO();
        public AttributeType AttributeType { get; set; }
        public float Value { get; set; }
        public float? Duration { get; set; }
        public float? Interval { get; set; }
    }
}