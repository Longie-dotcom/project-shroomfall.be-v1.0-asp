using Application.Interfaces.Repository.Relational;
using Domain.Definition.ItemDomain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Relational
{
    public class InventoryRepository : SQLGenericRepository<Inventory>, IInventoryRepository, IRelationalRepository
    {
        #region Attributes
        private readonly RelationalDB context;
        #endregion

        #region Properties
        #endregion

        public InventoryRepository(
            RelationalDB context) : base(
                context)
        {
            this.context = context;
        }

        #region Methods
        public async Task<List<Inventory>> GetAllWithDefaultItemsAsync()
        {
            return await context.Inventories
                .Include(i => i.DefaultItems)
                    .ThenInclude(e => e.Item)
                .AsNoTracking()
                .ToListAsync();
        }
        #endregion
    }
}