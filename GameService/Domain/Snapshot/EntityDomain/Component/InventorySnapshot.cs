using Domain.Abstraction;
using Domain.Snapshot.MetaDomain;

namespace Domain.Snapshot.EntityDomain.Component
{
    public class InventorySnapshot : ComponentSnapshot
    {
        public List<ItemSnapshot> Items { get; set; } = new();
    }
}