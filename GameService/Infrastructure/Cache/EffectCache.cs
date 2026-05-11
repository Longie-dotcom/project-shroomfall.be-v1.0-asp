using Application.Interfaces.Cache;
using Domain.Definition.AttributeDomain;

namespace Infrastructure.Cache
{
    public class EffectCache : IEffectCache
    {
        #region Attributes
        private Dictionary<string, Effect> map = new();
        #endregion

        #region Properties
        #endregion

        public EffectCache()
        {

        }

        #region Methods
        public void Load(
            IEnumerable<Effect> data)
        {
            map = data.ToDictionary(x => x.ID);
        }

        public IReadOnlyCollection<Effect> GetAll()
        {
            return map.Values.ToList();
        }

        public Effect? Get(
            string id)
        {
            return map.TryGetValue(id, out var effect)
                ? effect
                : null;
        }
        #endregion
    }
}