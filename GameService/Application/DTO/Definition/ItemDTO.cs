using Domain.Definition.ItemDomain.Enum;

namespace Application.DTO.Definition
{
    public class InventoryDefinitionDTO
    {
        public string ID { get; set; } = string.Empty;
        public InventoryType Type { get; set; }
        public LocalizedTextDTO LocalizedText { get; set; } = new LocalizedTextDTO();
        public int SlotCount { get; set; }
    }

    public class InventoryItemDefinitionDTO
    {
        public string InventoryID { get; set; } = string.Empty;
        public string ItemID { get; set; } = string.Empty;
        public int Amount { get; set; }
        public ItemQuality Quality { get; set; }
    }

    public class ItemDefinitionDTO
    {
        public string ID { get; set; } = string.Empty;
        public ItemType Type { get; set; }
        public LocalizedTextDTO LocalizedText { get; set; } = new LocalizedTextDTO();
        public ItemCategory Category { get; set; }
        public int? Durability { get; set; }
        public bool Stackable { get; set; }
        public string CharacteristicID { get; set; } = string.Empty;
        public string? ProjectileID { get; set; }
        public string? AreaEffectID { get; set; }
        public string? WorldObjectID { get; set; }
    }

    public class ItemEffectDefinitionDTO
    {
        public string ItemID { get; set; } = string.Empty;
        public string EffectID { get; set; } = string.Empty;
    }
}