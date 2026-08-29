using Application.Interface.Cache.MetaDomain;
using Contract.DTO.Definition.MetaDomain;

namespace Infrastructure.Cache.MetaDomain
{
    public class ItemCache : IItemCache
    {
        #region Attributes
        private Dictionary<string, ItemDefinitionDTO> byId = new();
        #endregion

        #region Properties
        #endregion

        public ItemCache() { }

        #region Methods
        public void Load(
            List<ItemDefinitionDTO> data)
        {
            byId = data.ToDictionary(x => x.Id, x => x);
        }

        public IEnumerable<ItemDefinitionDTO> GetAll()
        {
            return byId.Values;
        }

        public ItemDefinitionDTO? Get(
            string id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }
        #endregion
    }
}