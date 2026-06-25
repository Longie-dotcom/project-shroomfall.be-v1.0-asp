using Application.Interfaces.Repository.Relational;
using Domain.Definition.EntityDomain.Component;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Relational
{
    public class CharacteristicDefinitionRepository : SQLDefinitionRepository<CharacteristicDefinition>, ICharacteristicDefinitionRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public CharacteristicDefinitionRepository(RelationalDB context) : base(context) { }

        #region Methods
        public override async Task<IEnumerable<CharacteristicDefinition>> GetAllAsync()
        {
            return await dbSet
                .Include(l => l.AttributeValues)
                .ThenInclude(a => a.AttributeGrowthValues)
                .ToListAsync();
        }

        public override async Task<CharacteristicDefinition?> GetByEntityIdAsync(
            string entityId)
        {
            return await dbSet
                .Include(l => l.AttributeValues)
                    .ThenInclude(a => a.AttributeGrowthValues)
                .FirstOrDefaultAsync(c => c.EntityDefinitionID == entityId);
        }

        /// <summary>
        /// Explicitly inserts a collection of Level 1 child attributes into the database.
        /// </summary>
        public async Task SaveAttributeValuesAsync(
            IEnumerable<AttributeValue> attributeValues)
        {
            if (attributeValues == null || !attributeValues.Any()) return;

            await context.Set<AttributeValue>().AddRangeAsync(attributeValues);
        }

        /// <summary>
        /// Explicitly inserts a collection of Level 2 deep nested progression growth points into the database.
        /// </summary>
        public async Task SaveAttributeGrowthValuesAsync(
            IEnumerable<AttributeGrowthValue> growthValues)
        {
            if (growthValues == null || !growthValues.Any()) return;

            await context.Set<AttributeGrowthValue>().AddRangeAsync(growthValues);
        }

        /// <summary>
        /// Purges all existing attributes attached to a blueprint profile and swaps them with an overwritten dataset.
        /// </summary>
        public async Task ReplaceAttributeValuesAsync(
            Guid characteristicId,
            IEnumerable<AttributeValue> newValues)
        {
            var oldValues = await context.Set<AttributeValue>()
                .Where(v => v.CharacteristicDefinitionID == characteristicId)
                .ToListAsync();

            if (oldValues.Any())
            {
                context.Set<AttributeValue>().RemoveRange(oldValues);
            }

            if (newValues != null && newValues.Any())
            {
                await context.Set<AttributeValue>().AddRangeAsync(newValues);
            }
        }

        /// <summary>
        /// Purges progression point curves for a distinct attribute node and maps a fresh dataset over it.
        /// </summary>
        public async Task ReplaceAttributeGrowthValuesAsync(
            Guid attributeValueId,
            IEnumerable<AttributeGrowthValue> newGrowths)
        {
            var oldGrowths = await context.Set<AttributeGrowthValue>()
                .Where(g => g.AttributeValueID == attributeValueId)
                .ToListAsync();

            if (oldGrowths.Any())
            {
                context.Set<AttributeGrowthValue>().RemoveRange(oldGrowths);
            }

            if (newGrowths != null && newGrowths.Any())
            {
                await context.Set<AttributeGrowthValue>().AddRangeAsync(newGrowths);
            }
        }
        #endregion
    }
}