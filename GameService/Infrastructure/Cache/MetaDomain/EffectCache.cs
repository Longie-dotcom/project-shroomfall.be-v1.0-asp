using Application.Interfaces.Cache.MetaDomain;
using Domain.Definition.MetaDomain;

namespace Infrastructure.Cache.MetaDomain
{
    public class EffectCache : IEffectCache
    {
        #region Attributes
        private Dictionary<string, EffectDefinition> byId = new();
        #endregion

        #region Properties
        #endregion

        public EffectCache() { }

        #region Methods
        public void Load(
            List<EffectDefinition> data)
        {
            byId = data.ToDictionary(x => x.ID, x => x);
        }

        public IEnumerable<EffectDefinition> GetAll()
        {
            return byId.Values;
        }

        public EffectDefinition? Get(
            string id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }
        #endregion
    }
}