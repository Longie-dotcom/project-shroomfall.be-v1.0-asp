using Contract.DTO.Definition.EntityDomain.Component;

namespace Application.Interface.Cache.EntityDomain.Component
{
    public interface IInventoryCache
    {
        void Load(
            List<InventoryDefinitionDTO> data);
        IEnumerable<InventoryDefinitionDTO> GetAll();
        InventoryDefinitionDTO? Get(
            Guid id);
        InventoryDefinitionDTO? GetByEntity(
            string entityDefinitionId);
    }
}
