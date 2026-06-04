using Application.Interfaces.Cache;
using Contract.Enum.AttributeDomain;
using Domain.Definition.AttributeDomain;

namespace Infrastructure.Cache
{
    public class AttributeValueCache : IAttributeValueCache
    {
        #region Attributes
        private Dictionary<string, List<AttributeValue>> map = new();
        #endregion

        #region Properties
        #endregion

        public AttributeValueCache()
        {

        }

        #region Methods
        public void Load(
            IEnumerable<AttributeValue> data)
        {
            map.Clear();

            map = data
            .GroupBy(x => x.CharacteristicID)
            .ToDictionary(g => g.Key, g => g.ToList());
        }

        public IReadOnlyCollection<AttributeValue> GetAll()
        {
            return map.Values.SelectMany(x => x).ToList();
        }

        public AttributeValue? Get(
            string characteristicId, 
            AttributeType type, 
            int level)
        {
            if (!map.TryGetValue(characteristicId, out var list))
                return null;

            return list.FirstOrDefault(x =>
                x.Type == type &&
                x.Level == level);
        }
        #endregion
    }
}