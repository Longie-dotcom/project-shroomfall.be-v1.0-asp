using Application.Interfaces.Repository.Relational;
using Domain.Definition.ItemDomain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Relational
{
    public class ItemRepository : SQLGenericRepository<Item>, IItemRepository, IRelationalRepository
    {
        #region Attributes
        private readonly RelationalDB context;
        #endregion

        #region Properties
        #endregion

        public ItemRepository(
            RelationalDB context) : base(
                context)
        {
            this.context = context;
        }

        #region Methods
        public async Task<IEnumerable<Item>> GetAllWithEffectsAsync()
        {
            return await context.Items
                .Include(i => i.Effects)
                    .ThenInclude(e => e.Effect)
                .AsNoTracking()
                .ToListAsync();
        }
        #endregion
    }
}