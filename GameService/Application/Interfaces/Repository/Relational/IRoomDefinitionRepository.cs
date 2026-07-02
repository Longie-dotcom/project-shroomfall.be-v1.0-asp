using Application.Interfaces.Repository.Base;
using Domain.Definition.WorldDomain;

namespace Application.Interfaces.Repository.Relational
{
    public interface IRoomDefinitionRepository : ISQLGenericRepository<RoomDefinition>, IRelationalRepository
    {
        Task UpsertChildrenAsync(
            string roomDefinitionId,
            IEnumerable<Cell> cells,
            IEnumerable<EntitySpawnRule> spawnRules);
    }
}
