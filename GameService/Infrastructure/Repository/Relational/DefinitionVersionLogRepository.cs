using Application.Interfaces.Repository.Relational;
using Domain.Definition;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Relational
{
    public class DefinitionVersionLogRepository : SQLGenericRepository<DefinitionVersionLog>, IDefinitionVersionLogRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public DefinitionVersionLogRepository(RelationalDB context) : base(context) { }

        #region Methods
        public async Task<DefinitionVersionLog?> GetLatest(
            string key)
        {
            return await dbSet
                .Where(l => l.Key == key)
                .OrderByDescending(l => l.Version)
                .FirstOrDefaultAsync();
        }
        #endregion
    }
}