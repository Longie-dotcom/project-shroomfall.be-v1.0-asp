using Application.Interface.Cache.MetaDomain;
using Contract.DTO.Definition.MetaDomain;

namespace Infrastructure.Cache.MetaDomain
{
    public class EffectCache : IEffectCache
    {
        #region Attributes
        private Dictionary<string, EffectDefinitionDTO> byId = new();
        #endregion

        #region Properties
        #endregion

        public EffectCache() { }

        #region Methods
        public void Load(
            List<EffectDefinitionDTO> data)
        {
            byId = data.ToDictionary(x => x.Id, x => x);
        }

        public IEnumerable<EffectDefinitionDTO> GetAll()
        {
            return byId.Values;
        }

        public EffectDefinitionDTO? Get(
            string id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }
        #endregion
    }
}