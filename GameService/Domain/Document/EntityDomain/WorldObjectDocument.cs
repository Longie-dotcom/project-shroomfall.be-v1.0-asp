using Domain.Document.ItemDomain;

namespace Domain.Document.EntityDomain
{
    public class WorldObjectDocument : EntityDocument
    {
        public InventoryDocument? Inventory { get; set; } = new();
    }
}