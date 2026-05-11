using Domain.Definition.ItemDomain;

namespace Application.Interfaces.Cache
{
    public interface IItemCache
    {
        void Load(
            IEnumerable<Item> data);
        IReadOnlyCollection<Item> GetAll();
        Item? Get(
            string id);
    }
}