using Application.Interfaces.Repository.Base;
using Domain.Definition;

namespace Application.Interfaces.Repository.Relational
{
    public interface IDefinitionVersionLogRepository : ISQLGenericRepository<DefinitionVersionLog>, IRelationalRepository
    {
        Task<DefinitionVersionLog?> GetLatest(
            string key);
    }
}
