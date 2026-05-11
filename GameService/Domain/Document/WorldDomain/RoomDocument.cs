using Domain.Abstraction;

namespace Domain.Document.WorldDomain
{
    public class RoomDocument : IDocumentObject
    {
        public string ID { get; set; } = string.Empty;
        public string DefinitionID { get; set; } = string.Empty;
        public string OwnerID { get; set; } = string.Empty;
    }
}