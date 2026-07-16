using Application.Interfaces.Repository.Relational;
using Contract.Enum.MetaDomain.Item;
using Contract.Enum.WorldDomain;
using Domain.Definition.MetaDomain;
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
            // Handle Cells collection replacement
            var oldCells = await context.Set<Cell>()
                .Where(x => x.RoomDefinitionID == roomDefinitionId)
                .ToListAsync();

            if (oldCells.Any())
            {
                context.Set<Cell>().RemoveRange(oldCells);
            }

            if (cells != null && cells.Any())
            {
                await context.Set<Cell>().AddRangeAsync(cells);
            }

            // Handle EntitySpawnRules collection replacement
            var oldRules = await context.Set<EntitySpawnRule>()
                .Where(x => x.RoomDefinitionID == roomDefinitionId)
                .ToListAsync();

            if (oldRules.Any())
            {
                context.Set<EntitySpawnRule>().RemoveRange(oldRules);
            }

            if (spawnRules != null && spawnRules.Any())
            {
                await context.Set<EntitySpawnRule>().AddRangeAsync(spawnRules);
            }
        }
        #endregion
    }
}