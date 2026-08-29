using Application.Interface.Cache.EntityDomain.Component;
using Contract.DTO.Definition.EntityDomain.Component;
using Domain.DomainException;
using ResponseCode;

namespace Infrastructure.Cache.EntityDomain.Component
{
    public class LifetimeCache : ILifetimeCache
    {
        #region Attributes
        private Dictionary<Guid, LifetimeDefinitionDTO> byId = new();
        private Dictionary<string, LifetimeDefinitionDTO> byEntityId = new();
        #endregion

        #region Properties
        #endregion

        public LifetimeCache() { }

        #region Methods
        public void Load(
            List<LifetimeDefinitionDTO> data)
        {
            byId = data.ToDictionary(x => x.ID!.Value, x => x);

            byEntityId.Clear();

            foreach (var item in data)
            {
                var key = item.EntityDefinitionID;

                if (byEntityId.TryGetValue(key, out var existing))
                    throw new InternalException(
                        InfrastructureCode.LifetimeCacheCode.DuplicateLifetimeComponent,
                        $"Duplicate component detected for EntityDefinitionID '{key}' in {typeof(LifetimeCache).Name}. Existing: {existing.ID}, New: {item.ID}");

                byEntityId[key] = item;
            }
        }

        public IEnumerable<LifetimeDefinitionDTO> GetAll()
        {
            return byId.Values;
        }

        public LifetimeDefinitionDTO? Get(
            Guid id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }

        public LifetimeDefinitionDTO? GetByEntity(
            string entityDefinitionId)
        {
            byEntityId.TryGetValue(entityDefinitionId, out var value);
            return value;
        }
        #endregion
    }
}