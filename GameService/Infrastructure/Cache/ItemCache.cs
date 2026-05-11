using Application.Interfaces.Cache;
using Domain.Definition.ItemDomain;

namespace Infrastructure.Cache
{
    public class ItemCache : IItemCache
    {
        #region Attributes
        private Dictionary<string, Item> map = new();
        #endregion

        #region Properties
        #endregion

        public ItemCache()
        {

        }

        #region Methods
        public void Load(
            IEnumerable<Item> data)
        {
            map = data.ToDictionary(x => x.ID);
        }

        public IReadOnlyCollection<Item> GetAll()
        {
            return map.Values.ToList();
        }

        public Item? Get(
            string id)
        {
            return map.TryGetValue(id, out var item)
                ? item
                : null;
        }
        #endregion
    }
}