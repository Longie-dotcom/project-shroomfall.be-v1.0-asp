using Application.Interfaces.Cache.WorldDomain;
using Domain.Definition.WorldDomain;

namespace Infrastructure.Cache.WorldDomain
{
    public class CombatRunCache : ICombatRunCache
    {
        #region Attributes
        private Dictionary<string, CombatRunDefinition> map = new();
        #endregion

        #region Properties
        #endregion

        public CombatRunCache() { }

        #region Methods
        public void Load(
            IEnumerable<CombatRunDefinition> data)
        {
            map = data.ToDictionary(x => x.ID, x => x);
        }

        public IReadOnlyCollection<CombatRunDefinition> GetAll()
        {
            return map.Values.ToList();
        }

        public CombatRunDefinition? Get(
            string id)
        {
            return map.TryGetValue(id, out var item)
                ? item
                : null;
        }
        #endregion
    }
}