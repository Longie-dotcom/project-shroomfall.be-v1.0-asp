using Contract.Enum.ItemDomain;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Document.ItemDomain
{
    public class ItemDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string ID { get; set; } = string.Empty;
        public string DefinitionID { get; set; } = string.Empty;
        public int Count { get; set; }
        public int? CurrentDurability { get; set; }
        public ItemQuality Quality { get; set; }
    }
}