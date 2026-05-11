using Application.DTO.Common;
using Domain.Runtime.EntityDomain.Enum;

namespace Application.DTO.Runtime
{
    public class EquipmentRuntimeDTO
    {
        public EquipmentSlot EquipmentSlot { get; set; }
        public ItemRuntimeDTO? Item { get; set; } = new ItemRuntimeDTO();
    }

    public class EntityRuntimeDTO
    {
        public string ID { set; get; } = string.Empty;
        public string DefinitionID { set; get; } = string.Empty;
        public string RoomSpatialID { get; set; } = string.Empty;
        public int LayerZ { get; set; }
        public Vector2DTO Position { get; set; } = new Vector2DTO();
        public Vector2DTO Direction { get; set; } = new Vector2DTO();
        public AppearanceRuntimeDTO Appearance { get; set; } = new AppearanceRuntimeDTO();
    }

    public class WorldObjectRuntimeDTO : EntityRuntimeDTO
    {
        public InventoryRuntimeDTO? Inventory { get; set; } = new InventoryRuntimeDTO();
        public string? RoomSpatialReferenceID { get; set; } = string.Empty;
    }

    public class CreatureRuntimeDTO : EntityRuntimeDTO
    {
        public CharacteristicRuntimeDTO Characteristic { get; set; } = new CharacteristicRuntimeDTO();
        public InventoryRuntimeDTO Inventory { get; set; } = new InventoryRuntimeDTO();
        public int Level { get; private set; }
        public List<EffectRuntimeDTO> ActiveEffects { get; set; } = new();
        public List<EquipmentRuntimeDTO> Equipment { get; set; } = new();
    }

    public class PlayerRuntimeDTO : CreatureRuntimeDTO
    {
        public string UserID { get; set; } = string.Empty;
        public PlayerAppearanceRuntimeDTO PlayerAppearance { get; set; } = new PlayerAppearanceRuntimeDTO();
    }

    public class AppearanceRuntimeDTO
    {
        public string SkinID { get; set; } = string.Empty;
        public HSVDTO SkinColor { get; set; } = new HSVDTO();
    }

    public class PlayerAppearanceRuntimeDTO : AppearanceRuntimeDTO
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
}