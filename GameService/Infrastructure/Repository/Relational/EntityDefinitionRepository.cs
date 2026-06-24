using Application.Interfaces.Repository.Relational;
using Domain.Definition.EntityDomain;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Relational
{
    public class EntityDefinitionRepository : SQLGenericRepository<EntityDefinition>, IEntityDefinitionRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public EntityDefinitionRepository(RelationalDB context) : base(context) { }

        #region Methods
        public async Task<(IEnumerable<EntityDefinition> Items, int TotalCount)> GetPagedDefinitionsAsync(
            string? searchTerm,
            int pageNumber,
            int pageSize)
        {
            // 1. Maintain a high-performance un-evaluated read stream
            var query = dbSet.AsNoTracking().AsQueryable();

            // 2. Filter on the server side
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(e => e.ID.ToLower().Contains(term));
            }

            // 3. Count matching profiles inside database engine records indexes 
            int totalCount = await query.CountAsync();

            // 4. Pull only the requested row slice across the network pipe
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
        #endregion
    }
}