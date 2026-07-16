using Application.Interfaces.Repository.Base;
using Contract.Enum.WorldDomain;
using Domain.Definition.WorldDomain;

namespace Application.Interfaces.Repository.Relational
{
    public interface IRoomDefinitionRepository : ISQLGenericRepository<RoomDefinition>, IRelationalRepository
    {
        Task<(IEnumerable<RoomDefinition> Items, int TotalCount)> GetPagedDefinitionsAsync(
            string? searchTerm,
            RoomType? type,
            int pageNumber,
            int pageSize);
        Task UpsertChildrenAsync(
            string roomDefinitionId,
            IEnumerable<Cell> cells,
            IEnumerable<EntitySpawnRule> spawnRules);
    }
}
