using Domain.Definition.ItemDomain.Enum;

namespace Application.DTO.Runtime
{
    public class InventoryRuntimeDTO
    {
        public string ID { get; set; } = string.Empty;
        public string DefinitionID { get; set; } = string.Empty;
        public List<ItemRuntimeDTO> Items { get; set; } = new();
    }

    public class ItemRuntimeDTO
    {
        public string ID { get; set; } = string.Empty;
        public string DefinitionID { get; set; } = string.Empty;
        public int Count { get; set; }
        public int? CurrentDurability { get; set; }
        public ItemQuality Quality { get; set; }
    }
}