using Application.Interfaces.Cache.MetaDomain;
using Domain.Definition.MetaDomain;

namespace Infrastructure.Cache.MetaDomain
{
    public class ItemCache : IItemCache
    {
        #region Attributes
        private Dictionary<string, ItemDefinition> byId = new();
        #endregion

        #region Properties
        #endregion

        public ItemCache() { }

        #region Methods
        public void Load(
            List<ItemDefinition> data)
        {
            byId = data.ToDictionary(x => x.ID, x => x);
        }

        public IEnumerable<ItemDefinition> GetAll()
        {
            return byId.Values;
        }

        public ItemDefinition? Get(
            string id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }
        #endregion
    }
}