using Application.Interfaces.Cache;
using Domain.Definition.ItemDomain;

namespace Infrastructure.Cache
{
    public class InventoryCache : IInventoryCache
    {
        #region Attributes
        private Dictionary<string, Inventory> map = new();
        #endregion

        #region Properties
        #endregion

        public InventoryCache()
        {

        }

        #region Methods
        public void Load(
            IEnumerable<Inventory> data)
        {
            map = data.ToDictionary(x => x.ID);
        }

        public IReadOnlyCollection<Inventory> GetAll()
        {
            return map.Values.ToList();
        }

        public Inventory? Get(
            string id)
        {
            return map.TryGetValue(id, out var inventory)
                ? inventory
                : null;
        }
        #endregion
    }
}