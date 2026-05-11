using Domain.Definition.AttributeDomain.Enum;

namespace Application.DTO.Runtime
{
    public class AttributeValueRuntimeDTO
    {
        public AttributeType AttributeType { get; set; }
        public float Value { get; set; }
    }

    public class CharacteristicRuntimeDTO
    {
        public string ID { get; set; } = string.Empty;
        public string DefinitionID { get; set; } = string.Empty;
        public List<AttributeValueRuntimeDTO> Cores { get; set; } = new();
        public List<AttributeValueRuntimeDTO> Vitals { get; set; } = new();
    }

    public class EffectRuntimeDTO
    {
        public string ID { get; set; } = string.Empty;
        public string DefinitionID { get; set; } = string.Empty;
        public float? RemainingTime { get; set; }
        public string? SourceItemInstanceID { get; set; }
    }
}