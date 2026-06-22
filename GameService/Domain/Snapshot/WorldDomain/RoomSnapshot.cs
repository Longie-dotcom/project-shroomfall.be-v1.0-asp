using Domain.Abstraction;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Snapshot.WorldDomain
{
    public class RoomSnapshot : ISnapshot
    {
        [BsonId]
        public string ID { get; set; } = string.Empty;
        public string DefinitionID { get; set; } = string.Empty;
        public string? OwnerID { get; set; }
    }
}