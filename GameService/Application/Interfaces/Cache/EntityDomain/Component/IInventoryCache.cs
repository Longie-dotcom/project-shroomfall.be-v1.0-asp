using Domain.Definition.EntityDomain.Component;

namespace Application.Interfaces.Cache.EntityDomain.Component
{
    public interface IInventoryCache
    {
        void Load(
            List<InventoryDefinition> data);
        IEnumerable<InventoryDefinition> GetAll();
        InventoryDefinition? Get(
            Guid id);
        InventoryDefinition? GetByEntity(
            string entityDefinitionId);
    }
}
