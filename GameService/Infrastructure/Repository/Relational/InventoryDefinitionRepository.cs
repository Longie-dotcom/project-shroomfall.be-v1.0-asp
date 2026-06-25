using Application.Interfaces.Repository.Relational;
using Domain.Definition.EntityDomain.Component;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Relational
{
    public class InventoryDefinitionRepository : SQLDefinitionRepository<InventoryDefinition>, IInventoryDefinitionRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public InventoryDefinitionRepository(RelationalDB context) : base(context) { }

        #region Methods
        public override async Task<InventoryDefinition?> GetByEntityIdAsync(
            string entityId)
        {
            return await dbSet
                .Include(l => l.DefaultItems)
                .FirstOrDefaultAsync(i => i.EntityDefinitionID == entityId);
        }

        public override async Task<IEnumerable<InventoryDefinition>> GetAllAsync()
        {
            return await dbSet
                .Include(l => l.DefaultItems)
                .ToListAsync();
        }

        /// <summary>
        /// Explicitly inserts a collection of child inventory entries into the database.
        /// </summary>
        public async Task SaveDefaultItemsAsync(
            IEnumerable<InventoryEntry> defaultItems)
        {
            if (defaultItems == null || !defaultItems.Any()) return;

            await context.Set<InventoryEntry>().AddRangeAsync(defaultItems);
        }

        /// <summary>
        /// Purges all existing default items attached to an inventory template and swaps them with an overwritten dataset.
        /// </summary>
        public async Task ReplaceDefaultItemsAsync(
            Guid inventoryDefinitionId,
            IEnumerable<InventoryEntry> newItems)
        {
            var oldItems = await context.Set<InventoryEntry>()
                .Where(i => i.InventoryDefinitionID == inventoryDefinitionId)
                .ToListAsync();

            if (oldItems.Any())
            {
                context.Set<InventoryEntry>().RemoveRange(oldItems);
            }

            if (newItems != null && newItems.Any())
            {
                await context.Set<InventoryEntry>().AddRangeAsync(newItems);
            }
        }
        #endregion
    }
}