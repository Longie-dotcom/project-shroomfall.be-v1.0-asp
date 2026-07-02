using Application.Interfaces.Repository.Relational;
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

        public async Task UpsertChildrenAsync(
            string roomDefinitionId,
            IEnumerable<Cell> cells,
            IEnumerable<EntitySpawnRule> spawnRules)
        {
            // 1. Handle Cells collection replacement
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

            // 2. Handle EntitySpawnRules collection replacement
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