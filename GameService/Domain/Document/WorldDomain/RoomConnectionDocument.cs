using Domain.Abstraction;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Document.WorldDomain
{
    public class RoomConnectionDocument : IDocumentObject
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string ID { get; set; } = string.Empty;
        public string DefinitionID { get; set; } = string.Empty;
        public string SourceRoomSpatialID { get; set; } = string.Empty;
        public string SourceEntityInstanceID { get; set; } = string.Empty;
        public string? DestinationRoomSpatialID { get; set; } = string.Empty;
        public string? DestinationEntityInstanceID { get; set; } = string.Empty;
    }
}