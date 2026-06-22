using Domain.Abstraction;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Snapshot.WorldDomain
{
    public class RoomConnectionSnapshot : ISnapshot
    {
        [BsonId]
        public string ID { get; set; } = string.Empty;
        public string DefinitionID { get; set; } = string.Empty;
        public string SourceRoomSpatialID { get; set; } = string.Empty;
        public string SourceEntityInstanceID { get; set; } = string.Empty;
        public string? DestinationRoomSpatialID { get; set; }
        public string? DestinationEntityInstanceID { get; set; }
        public string? ReverseConnectionID { get; set; }
    }
}