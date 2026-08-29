using Application.Interface.Cache.EntityDomain.Component;
using Contract.DTO.Definition.EntityDomain.Component;
using Domain.DomainException;
using ResponseCode;

namespace Infrastructure.Cache.EntityDomain.Component
{
    public class InventoryCache : IInventoryCache
    {
        #region Attributes
        private Dictionary<Guid, InventoryDefinitionDTO> byId = new();
        private Dictionary<string, InventoryDefinitionDTO> byEntityId = new();
        #endregion

        #region Properties
        #endregion

        public InventoryCache() { }

        #region Methods
        public void Load(
            List<InventoryDefinitionDTO> data)
        {
            byId = data.ToDictionary(x => x.ID!.Value, x => x);

            byEntityId.Clear();

            foreach (var item in data)
            {
                var key = item.EntityDefinitionID;

                if (byEntityId.TryGetValue(key, out var existing))
                    throw new InternalException(
                        InfrastructureCode.InventoryCacheCode.DuplicateInventoryComponent,
                        $"Duplicate component detected for EntityDefinitionID '{key}' in {typeof(InventoryCache).Name}. Existing: {existing.ID}, New: {item.ID}");

                byEntityId[key] = item;
            }
        }

        public IEnumerable<InventoryDefinitionDTO> GetAll()
        {
            return byId.Values;
        }

        public InventoryDefinitionDTO? Get(
            Guid id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }

        public InventoryDefinitionDTO? GetByEntity(
            string entityDefinitionId)
        {
            byEntityId.TryGetValue(entityDefinitionId, out var value);
            return value;
        }
        #endregion
    }
}