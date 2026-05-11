using Application.Interfaces.Cache;
using Domain.Definition.AttributeDomain;

namespace Infrastructure.Cache
{
    public class CharacteristicCache : ICharacteristicCache
    {
        #region Attributes
        private Dictionary<string, Characteristic> map = new();
        #endregion

        #region Properties
        #endregion

        public CharacteristicCache()
        {

        }

        #region Methods
        public void Load(
            IEnumerable<Characteristic> data)
        {
            map = data.ToDictionary(x => x.ID);
        }

        public IReadOnlyCollection<Characteristic> GetAll()
        {
            return map.Values.ToList();
        }

        public Characteristic? Get(
            string id)
        {
            return map.TryGetValue(id, out var characteristic)
                ? characteristic
                : null;
        }
        #endregion
    }
}