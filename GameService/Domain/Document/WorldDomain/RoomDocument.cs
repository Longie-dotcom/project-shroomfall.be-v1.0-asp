using Domain.Abstraction;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Document.WorldDomain
{
    public class RoomDocument : IDocumentObject
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string ID { get; set; } = string.Empty;
        public string DefinitionID { get; set; } = string.Empty;
        public string? OwnerID { get; set; } = string.Empty;
    }
}