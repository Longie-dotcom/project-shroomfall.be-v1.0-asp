using Application.Interface.Cache.WorldDomain;
using Contract.DTO.Definition.WorldDomain;

namespace Infrastructure.Cache.WorldDomain
{
    public class CombatRunCache : ICombatRunCache
    {
        #region Attributes
        private Dictionary<string, CombatRunDefinitionDTO> map = new();
        #endregion

        #region Properties
        #endregion

        public CombatRunCache() { }

        #region Methods
        public void Load(
            IEnumerable<CombatRunDefinitionDTO> data)
        {
            map = data.ToDictionary(x => x.Id, x => x);
        }

        public IReadOnlyCollection<CombatRunDefinitionDTO> GetAll()
        {
            return map.Values.ToList();
        }

        public CombatRunDefinitionDTO? Get(
            string id)
        {
            return map.TryGetValue(id, out var item)
                ? item
                : null;
        }
        #endregion
    }
}