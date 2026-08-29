using Application.Interface.Cache.EntityDomain;
using Contract.DTO.Definition.EntityDomain.Component;

namespace Infrastructure.Cache.EntityDomain
{
    public class EntityCache : IEntityCache
    {
        #region Attributes
        private Dictionary<string, EntityDefinitionDTO> entities = new();
        #endregion

        #region Properties
        #endregion

        public EntityCache() { }

        #region Methods
        public void Load(
            List<EntityDefinitionDTO> data)
        {
            entities.Clear();

            entities = data.ToDictionary(
                x => x.Id,
                x => x);
        }

        public IEnumerable<EntityDefinitionDTO> GetAll()
        {
            return entities.Values;
        }

        public EntityDefinitionDTO? Get(
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