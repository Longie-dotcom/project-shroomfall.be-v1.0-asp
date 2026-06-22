using Contract.Enum.MetaDomain.Item;
using Domain.Abstraction;

namespace Domain.Snapshot.EntityDomain.Component
{
    public class InventorySnapshot : ComponentSnapshot
    {
        public List<ItemSnapshot> Items { get; set; } = new();
    }

    public class ItemSnapshot : IItemStateContract
    {
        public string ID { get; set; } = string.Empty;
        public string DefinitionID { get; set; } = string.Empty;
        public int Amount { get; set; }
        public ItemQuality Quality { get; set; }
        public int? Durability { get; set; }
    }
}