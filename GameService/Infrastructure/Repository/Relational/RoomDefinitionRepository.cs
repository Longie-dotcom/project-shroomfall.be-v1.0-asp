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
        #endregion
    }
}