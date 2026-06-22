using Application.Interfaces.Repository.Relational;
using Domain.Definition.EntityDomain.Component;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Relational
{
    public class SpawnDefinitionRepository : SQLGenericRepository<SpawnDefinition>, ISpawnDefinitionRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public SpawnDefinitionRepository(RelationalDB context) : base(context) { }

        #region Methods
        public override async Task<IEnumerable<SpawnDefinition>> GetAllAsync()
        {
            return await dbSet
                .Include(l => l.SpawnEntries)
                .ToListAsync();
        }
        #endregion
    }
}