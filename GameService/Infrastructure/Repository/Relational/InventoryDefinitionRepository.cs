using Application.Interfaces.Repository.Relational;
using Domain.Definition.EntityDomain.Component;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Relational
{
    public class InventoryDefinitionRepository : SQLGenericRepository<InventoryDefinition>, IInventoryDefinitionRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public InventoryDefinitionRepository(RelationalDB context) : base(context) { }

        #region Methods
        public override async Task<IEnumerable<InventoryDefinition>> GetAllAsync()
        {
            return await dbSet
                .Include(l => l.DefaultItems)
                .ToListAsync();
        }
        #endregion
    }
}