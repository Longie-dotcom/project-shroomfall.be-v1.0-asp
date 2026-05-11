using Domain.Definition.ItemDomain;

namespace Application.Interfaces.Cache
{
    public interface IInventoryCache
    {
        void Load(
            IEnumerable<Inventory> data);
        IReadOnlyCollection<Inventory> GetAll();
        Inventory? Get(
            string id);
    }
}