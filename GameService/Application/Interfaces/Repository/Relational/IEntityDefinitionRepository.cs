using Application.Interfaces.Repository.Base;
using Contract.Enum.EntityDomain;
using Domain.Definition.EntityDomain;

namespace Application.Interfaces.Repository.Relational
{
    public interface IEntityDefinitionRepository : ISQLGenericRepository<EntityDefinition>, IRelationalRepository
    {
        Task<(IEnumerable<EntityDefinition> Items, int TotalCount)> GetPagedDefinitionsAsync(
            string? searchTerm,
            EntityType? type,
            int pageNumber,
            int pageSize);
    }
}
