using Application.Interfaces.Cache.EntityDomain.Component;
using Domain.Definition.EntityDomain.Component;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Infrastructure.Cache.EntityDomain.Component
{
    public class InventoryCache : IInventoryCache
    {
        #region Attributes
        private Dictionary<Guid, InventoryDefinition> byId = new();
        private Dictionary<string, InventoryDefinition> byEntityId = new();
        #endregion

        #region Properties
        #endregion

        public InventoryCache() { }

        #region Methods
        public void Load(
            List<InventoryDefinition> data)
        {
            byId = data.ToDictionary(x => x.ID, x => x);

            byEntityId.Clear();

            foreach (var item in data)
            {
                var key = item.EntityDefinitionID;

                if (byEntityId.TryGetValue(key, out var existing))
                    throw new InternalException(
                        InfrastructureCode.InventoryCacheCode.DuplicateInventoryComponent,
                        $"Duplicate component detected for EntityDefinitionID '{key}' in {typeof(InventoryDefinition).Name}. Existing: {existing.ID}, New: {item.ID}");

                byEntityId[key] = item;
            }
        }

        public IEnumerable<InventoryDefinition> GetAll()
        {
            return byId.Values;
        }

        public InventoryDefinition? Get(
            Guid id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }

        public InventoryDefinition? GetByEntity(
            string entityDefinitionId)
        {
            byEntityId.TryGetValue(entityDefinitionId, out var value);
            return value;
        }
        #endregion
    }
}