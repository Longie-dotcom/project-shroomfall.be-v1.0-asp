using Application.Interfaces.Cache.EntityDomain.Component;
using Domain.Definition.EntityDomain.Component;
using Domain.DomainException;
using ResponseCode;

namespace Infrastructure.Cache.EntityDomain.Component
{
    public class LifetimeCache : ILifetimeCache
    {
        #region Attributes
        private Dictionary<Guid, LifetimeDefinition> byId = new();
        private Dictionary<string, LifetimeDefinition> byEntityId = new();
        #endregion

        #region Properties
        #endregion

        public LifetimeCache() { }

        #region Methods
        public void Load(
            List<LifetimeDefinition> data)
        {
            byId = data.ToDictionary(x => x.ID, x => x);

            byEntityId.Clear();

            foreach (var item in data)
            {
                var key = item.EntityDefinitionID;

                if (byEntityId.TryGetValue(key, out var existing))
                    throw new InternalException(
                        InfrastructureCode.LifetimeCacheCode.DuplicateLifetimeComponent,
                        $"Duplicate component detected for EntityDefinitionID '{key}' in {typeof(LifetimeDefinition).Name}. Existing: {existing.ID}, New: {item.ID}");

                byEntityId[key] = item;
            }
        }

        public IEnumerable<LifetimeDefinition> GetAll()
        {
            return byId.Values;
        }

        public LifetimeDefinition? Get(
            Guid id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }

        public LifetimeDefinition? GetByEntity(
            string entityDefinitionId)
        {
            byEntityId.TryGetValue(entityDefinitionId, out var value);
            return value;
        }
        #endregion
    }
}