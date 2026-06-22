using Application.Interfaces.Cache.EntityDomain.Component;
using Domain.Definition.EntityDomain.Component;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Infrastructure.Cache.EntityDomain.Component
{
    public class PortalCache : IPortalCache
    {
        #region Attributes
        private Dictionary<Guid, PortalDefinition> byId = new();
        private Dictionary<string, PortalDefinition> byEntityId = new();
        #endregion

        #region Properties
        #endregion

        public PortalCache() { }

        #region Methods
        public void Load(
            List<PortalDefinition> data)
        {
            byId = data.ToDictionary(x => x.ID, x => x);

            byEntityId.Clear();

            foreach (var item in data)
            {
                var key = item.EntityDefinitionID;

                if (byEntityId.TryGetValue(key, out var existing))
                    throw new InternalException(
                        InfrastructureCode.PortalCacheCode.DuplicatePortalComponent,
                        $"Duplicate component detected for EntityDefinitionID '{key}' in {typeof(PortalDefinition).Name}. Existing: {existing.ID}, New: {item.ID}");

                byEntityId[key] = item;
            }
        }

        public IEnumerable<PortalDefinition> GetAll()
        {
            return byId.Values;
        }

        public PortalDefinition? Get(
            Guid id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }

        public PortalDefinition? GetByEntity(
            string entityDefinitionId)
        {
            byEntityId.TryGetValue(entityDefinitionId, out var value);
            return value;
        }
        #endregion
    }
}