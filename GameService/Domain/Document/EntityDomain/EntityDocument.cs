using Domain.Abstraction;
using Domain.Common;
using Domain.Document.EntityDomain.Component;

namespace Domain.Document.EntityDomain
{
    public class EntityDocument : IDocumentObject
    {
        public string ID { get; set; } = string.Empty;
        public string DefinitionID { get; set; } = string.Empty;
        public string RoomSpatialID { get; set; } = string.Empty;
        public int LayerZ { get; set; }
        public Vector2Document Position { get; set; } = new();
        public Vector2Document Direction { get; set; } = new();
        public AppearanceDocument Appearance { get; set; } = new();

        // NOTE: CollisionShape no need to be persisted
        // because it is will be rebuilt based on definition
    }
}