using Contract.Enum.EntityDomain;
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
        public Vector2Document MovementVector { get; set; } = new();
        public bool PositionChangedThisFrame { get; set; }
        public bool WantsToMove { get; set; }
        public EntityDirection FacingDirection { get; set; }
        public EntityAction CurrentAction { get; set; }
        public bool IsActionLocked { get; set; }
        public AppearanceDocument Appearance { get; set; } = new();
        // NOTE: CollisionShape no need to be persisted
        // because it is will be rebuilt based on definition
    }
}