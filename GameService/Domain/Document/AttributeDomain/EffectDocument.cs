using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Document.AttributeDomain
{
    public class EffectDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string ID { get; set; } = string.Empty;
        public string DefinitionID { get; set; } = string.Empty;
        public float? RemainingTime { get; set; }
        public string? SourceItemInstanceID { get; set; }
    }
}