using Application.Interfaces.Repository.Base;
using Contract.Enum.MetaDomain.Item;
using Domain.Definition.MetaDomain;

namespace Application.Interfaces.Repository.Relational
{
    public interface IItemDefinitionRepository : ISQLGenericRepository<ItemDefinition>, IRelationalRepository
    {
        Task<(IEnumerable<ItemDefinition> Items, int TotalCount)> GetPagedDefinitionsAsync(
            string? searchTerm,
            ItemType? type,
            ItemCategory? category,
            int pageNumber,
            int pageSize);
    }
}
