using Application.DTO.Common;
using Domain.Definition.EntityDomain.Enum;

namespace Application.DTO.Definition
{
    public class EntityDefinitionDTO
    {
        public string ID { get; set; } = string.Empty;
        public EntityType Type { get; set; }
        public LocalizedTextDTO LocalizedText { get; set; } = new LocalizedTextDTO();
        public AppearanceDefinitionDTO Appearance { get; set; } = new AppearanceDefinitionDTO();
        public CollisionDefinitionDTO Collision { get; set; } = new CollisionDefinitionDTO();
    }

    public class AreaEffectDefinitionDTO : EntityDefinitionDTO
    {

    }

    public class ProjectileDefinitionDTO : EntityDefinitionDTO
    {

    }

    public class WorldObjectDefinitionDTO : EntityDefinitionDTO
    {
        public WorldObjectInteractionType InteractionType { get; set; }
        public bool IsInteractable { get; set; }
        public bool IsPickupable { get; set; }
        public string? InventoryID { get; set; }
        public string? RoomID { get; private set; }
    }

    public class CreatureDefinitionDTO : EntityDefinitionDTO
    {
        public string CharacteristicID { get; set; } = string.Empty;
        public string InventoryID { get; set; } = string.Empty;
        public int Level { get; set; }
    }

    public class PlayerDefinitionDTO : CreatureDefinitionDTO
    {
        public PlayerAppearanceDefinitionDTO PlayerAppearance { get; set; } = new PlayerAppearanceDefinitionDTO();
    }

    public class AppearanceDefinitionDTO
    {
        public string SkinID { get; set; } = string.Empty;
        public HSVDTO SkinColor { get; set; } = new HSVDTO();
    }

    public class PlayerAppearanceDefinitionDTO : AppearanceDefinitionDTO
    {
        public string HairID { get; set; } = string.Empty;
        public string GlassesID { get; set; } = string.Empty;
        public string ShirtID { get; set; } = string.Empty;
        public string PantID { get; set; } = string.Empty;
        public string ShoeID { get; set; } = string.Empty;
        public string EyesID { get; set; } = string.Empty;
        public HSVDTO HairColor { get; set; } = new HSVDTO();
        public HSVDTO PantColor { get; set; } = new HSVDTO();
        public HSVDTO EyeColor { get; set; } = new HSVDTO();
    }

    public class CollisionDefinitionDTO
    {
        public CollisionShapeType ShapeType { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Radius { get; set; }
        public bool IsBlocking { get; set; }
        public bool IsTrigger { get; set; }
    }
}