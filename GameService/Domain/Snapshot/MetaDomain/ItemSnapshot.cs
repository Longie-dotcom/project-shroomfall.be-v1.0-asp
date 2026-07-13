using Contract.Enum.MetaDomain.Item;

namespace Domain.Snapshot.MetaDomain
{
    public class ItemSnapshot
    {
        public string ID { get; set; } = string.Empty;
        public string DefinitionID { get; set; } = string.Empty;
        public int Amount { get; set; }
        public ItemQuality Quality { get; set; }
        public int? Durability { get; set; }
        public EquipmentSlot? EquippedSlot { get; set; }
    }
}