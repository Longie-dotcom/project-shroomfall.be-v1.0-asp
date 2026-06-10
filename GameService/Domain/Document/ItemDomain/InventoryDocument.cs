using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Document.ItemDomain
{
    public class InventoryDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string ID { get; set; } = string.Empty;
        public string DefinitionID { get; set; } = string.Empty;
        public List<ItemDocument> Items { get; set; } = new();
    }
}