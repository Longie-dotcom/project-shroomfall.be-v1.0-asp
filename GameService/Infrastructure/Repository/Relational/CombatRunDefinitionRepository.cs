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

        /// <summary>
        /// Replaces all floors belonging to a combat run definition.
        /// </summary>
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
        #endregion
    }
}