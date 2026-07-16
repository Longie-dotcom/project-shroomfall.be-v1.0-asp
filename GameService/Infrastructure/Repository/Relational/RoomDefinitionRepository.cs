using Application.Interfaces.Repository.Relational;
using Contract.Enum.WorldDomain;
using EFCore.BulkExtensions;
using Domain.Definition.WorldDomain;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Relational
{
    public class RoomDefinitionRepository : SQLGenericRepository<RoomDefinition>, IRoomDefinitionRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public RoomDefinitionRepository(RelationalDB context) : base(context) { }

        #region Methods
        public override async Task<IEnumerable<RoomDefinition>> GetAllAsync()
        {
            return await dbSet
                .Include(l => l.Cells)
                .Include(l => l.EntitySpawnRules)
                .ToListAsync();
        }

        public async Task<(IEnumerable<RoomDefinition> Items, int TotalCount)> GetPagedDefinitionsAsync(
            string? searchTerm,
            RoomType? type,
            int pageNumber,
            int pageSize)
        {
            // Create the queryable shell
            var query = dbSet.AsNoTracking().AsQueryable();

            // Conditionally append dynamic WHERE expressions
            if (type.HasValue)
            {
                query = query.Where(x => x.Type == type.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                // Matches against Primary ID Key or Owned Type Localization configuration keys
                query = query.Where(x =>
                    x.ID.ToLower().Contains(term) ||
                    x.Presentation.LocalizedText.NameKey.ToLower().Contains(term));
            }

            // Get total count tracking balance before executing pagination splits
            int totalCount = await query.CountAsync();

            // Slicing row constraints using database server execution bounds
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task UpsertChildrenAsync(
            string roomDefinitionId,
            IEnumerable<Cell> cells,
            IEnumerable<EntitySpawnRule> spawnRules)
        {
            // Executes immediately as a single SQL statement. Zero memory overhead.
            await context.Set<Cell>()
                .Where(x => x.RoomDefinitionID == roomDefinitionId)
                .ExecuteDeleteAsync();

            await context.Set<EntitySpawnRule>()
                .Where(x => x.RoomDefinitionID == roomDefinitionId)
                .ExecuteDeleteAsync();

            // Bypasses the EF Change Tracker entirely.
            if (cells != null && cells.Any())
            {
                await context.BulkInsertAsync(cells.ToList());
            }

            if (spawnRules != null && spawnRules.Any())
            {
                await context.BulkInsertAsync(spawnRules.ToList());
            }
        }
        #endregion
    }
}