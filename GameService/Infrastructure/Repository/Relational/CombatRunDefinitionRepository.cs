using Application.Interfaces.Repository.Relational;
using Domain.Definition.WorldDomain;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Relational
{
    public class CombatRunDefinitionRepository : SQLGenericRepository<CombatRunDefinition>, ICombatRunDefinitionRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public CombatRunDefinitionRepository(RelationalDB context) : base(context) { }

        #region Methods
        public override async Task<IEnumerable<CombatRunDefinition>> GetAllAsync()
        {
            return await dbSet
                .Include(x => x.Floors.OrderBy(f => f.Level))
                .ToListAsync();
        }

        public async Task UpsertFloorsAsync(
            string combatRunDefinitionId,
            IEnumerable<Floor> floors)
        {
            var oldFloors = await context.Set<Floor>()
                .Where(x => x.CombatRunDefinitionID == combatRunDefinitionId)
                .ToListAsync();

            if (oldFloors.Any())
            {
                context.Set<Floor>().RemoveRange(oldFloors);
            }

            if (floors != null && floors.Any())
            {
                await context.Set<Floor>().AddRangeAsync(floors);
            }
        }

        public async Task<(IEnumerable<CombatRunDefinition> Items, int TotalCount)> GetPagedDefinitionsAsync(
            string? searchTerm,
            int pageNumber,
            int pageSize)
        {
            var query = dbSet.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(x => x.ID.ToLower().Contains(term));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
        #endregion
    }
}