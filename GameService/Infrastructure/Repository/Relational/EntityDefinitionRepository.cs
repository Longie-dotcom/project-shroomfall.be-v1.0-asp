using Application.Interfaces.Repository.Relational;
using Contract.Enum.EntityDomain;
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
            EntityType? type, 
            int pageNumber,
            int pageSize)
        {
            // Maintain a high-performance un-evaluated read stream
            var query = dbSet.AsNoTracking().AsQueryable();

            if (type.HasValue)
            {
                query = query.Where(x => x.Type == type.Value);
            }

            // Filter on the server side
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(e => e.ID.ToLower().Contains(term));
            }

            // Count matching profiles inside database engine records indexes 
            int totalCount = await query.CountAsync();

            // Pull only the requested row slice across the network pipe
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
        #endregion
    }
}