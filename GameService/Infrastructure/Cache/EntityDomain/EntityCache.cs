using Application.Interfaces.Cache.EntityDomain;
using Domain.Definition.EntityDomain;

namespace Infrastructure.Cache.EntityDomain
{
    public class EntityCache : IEntityCache
    {
        #region Attributes
        private Dictionary<string, EntityDefinition> entities = new();
        #endregion

        #region Properties
        #endregion

        public EntityCache() { }

        #region Methods
        public void Load(
            List<EntityDefinition> data)
        {
            entities.Clear();

            entities = data.ToDictionary(
                x => x.ID,
                x => x);
        }

        public IEnumerable<EntityDefinition> GetAll()
        {
            return entities.Values;
        }

        public EntityDefinition? Get(
            string id)
        {
            entities.TryGetValue(
                id,
                out var entity);

            return entity;
        }
        #endregion
    }
}