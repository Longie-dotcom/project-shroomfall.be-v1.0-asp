using Application.Interfaces.Repository.Base;
using Domain.Definition.EntityDomain;

namespace Application.Interfaces.Repository.Relational
{
    public interface IEntityDefinitionRepository : ISQLGenericRepository<EntityDefinition>, IRelationalRepository
    {
        Task<(IEnumerable<EntityDefinition> Items, int TotalCount)> GetPagedDefinitionsAsync(
            string? searchTerm,
            int pageNumber,
            int pageSize);
    }
}
