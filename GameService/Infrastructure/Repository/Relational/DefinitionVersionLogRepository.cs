using Application.Interfaces.Repository.Relational;
using Domain.Other.VersionDomain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Relational
{
    public class DefinitionVersionLogRepository : SQLGenericRepository<DefinitionVersionLog>, IDefinitionVersionLogRepository, IRelationalRepository
    {
        #region Attributes
        private readonly RelationalDB context;
        #endregion

        #region Properties
        #endregion

        public DefinitionVersionLogRepository(
            RelationalDB context) : base(
                context)
        {
            this.context = context;
        }

        #region Methods
        public async Task<DefinitionVersionLog?> GetLatest(
            string key)
        {
            return await context.DefinitionVersionLogs
                .Where(x => x.Key == key)
                .OrderByDescending(x => x.Version)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
        #endregion
    }
}