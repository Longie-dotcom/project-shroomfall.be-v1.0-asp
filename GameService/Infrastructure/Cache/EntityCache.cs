using Application.Interfaces.Cache;
using Domain.Definition.EntityDomain;

namespace Infrastructure.Cache
{
    public class EntityCache : IEntityCache
    {
        #region Attributes
        private Dictionary<string, Entity> map = new();
        #endregion

        #region Properties
        #endregion

        public EntityCache()
        {

        }

        #region Methods
        public void Load(
            IEnumerable<Entity> data)
        {
            map = data.ToDictionary(x => x.ID, x => x);
        }

        public IReadOnlyCollection<Entity> GetAll()
        {
            return map.Values.ToList();
        }

        public T? Get<T>(
            string id) where T : Entity
        {
            if (!map.TryGetValue(id, out var entity))
                return null;

            if (entity is not T typed)
                return null;

            return typed;
        }
        #endregion
    }
}