using Domain.Abstraction;

namespace Domain.Document.WorldDomain
{
    public class RoomConnectionDocument : IDocumentObject
    {
        public string ID { get; set; } = string.Empty;
        public string DefinitionID { get; set; } = string.Empty;
        public string SourceRoomSpatialID { get; set; } = string.Empty;
        public string SourceEntityInstanceID { get; set; } = string.Empty;
        public string? DestinationRoomSpatialID { get; set; } = string.Empty;
        public string? DestinationEntityInstanceID { get; set; } = string.Empty;
    }
}