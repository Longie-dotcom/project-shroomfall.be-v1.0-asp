using Domain.Other.VersionDomain;

namespace Application.Interfaces.Repository.Relational
{
    public interface IDefinitionVersionLogRepository : ISQLGenericRepository<DefinitionVersionLog>, IRelationalRepository
    {
        Task<DefinitionVersionLog?> GetLatest(
            string key);
    }
}
