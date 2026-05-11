using Domain.Definition.WorldDomain;

namespace Application.Interfaces.Repository.Relational
{
    public interface IRoomRepository : ISQLGenericRepository<Room>, IRelationalRepository
    {
        Task<IReadOnlyList<Room>> GetAllWithCellsAndSpawnRulesAsync();
    }
}