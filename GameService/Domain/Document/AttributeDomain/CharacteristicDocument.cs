
using Contract.Enum.AttributeDomain;

namespace Domain.Document.AttributeDomain
{
    public class CharacteristicDocument
    {
        public string ID { get; set; } = string.Empty;
        public string DefinitionID { get; set; } = string.Empty;
        public Dictionary<AttributeType, float> Vitals { get; set; } = new Dictionary<AttributeType, float>();
        
        // NOTE: Core values no need to be persisted
        // because it is recalculated based on definition and entity active effects
    }
}