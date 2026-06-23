using Application.Interfaces.Repository.Base;
using Domain.Definition.EntityDomain.Component;

namespace Application.Interfaces.Repository.Relational
{
    public interface IInventoryDefinitionRepository : ISQLDefinitionRepository<InventoryDefinition>, IRelationalRepository
    {
        Task SaveDefaultItemsAsync(
            IEnumerable<InventoryEntry> defaultItems);
        Task ReplaceDefaultItemsAsync(
            Guid inventoryDefinitionId,
            IEnumerable<InventoryEntry> newItems);
    }
}
