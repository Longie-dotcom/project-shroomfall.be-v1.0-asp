using Application.Interfaces.Repository.Relational;
using Domain.Definition.WorldDomain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Relational
{
    public class RoomRepository : SQLGenericRepository<Room>, IRoomRepository, IRelationalRepository
    {
        #region Attributes
        private readonly RelationalDB context;
        #endregion

        #region Properties
        #endregion

        public RoomRepository(
            RelationalDB context) : base(
                context)
        {
            this.context = context;
        }

        #region Methods
        public async Task<IReadOnlyList<Room>> GetAllWithCellsAndSpawnRulesAsync()
        {
            return await context.Rooms
                .Include(r => r.Cells)
                .Include(r => r.EntitySpawnRules)
                    .ThenInclude(e => e.SpawnAreas)
                .AsSplitQuery()
                .AsNoTracking()
                .ToListAsync();
        }
        #endregion
    }
}