using Application.Interfaces.Repository.Relational;
using Contract.Enum.MetaDomain.Effect;
using Domain.Definition.MetaDomain;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Relational
{
    public class EffectDefinitionRepository : SQLGenericRepository<EffectDefinition>, IEffectDefinitionRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public EffectDefinitionRepository(RelationalDB context) : base(context) { }

        #region Methods
        public async Task<(IEnumerable<EffectDefinition> Items, int TotalCount)> GetPagedDefinitionsAsync(
            string? searchTerm,
            EffectType? type,
            AttributeType? attributeType,
            int pageNumber,
            int pageSize)
        {
            var query = dbSet.AsNoTracking().AsQueryable();

            if (type.HasValue)
            {
                query = query.Where(x => x.Type == type.Value);
            }

            if (attributeType.HasValue)
            {
                query = query.Where(x => x.AttributeType == attributeType.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(x =>
                    x.ID.ToLower().Contains(term) ||
                    x.Presentation.LocalizedText.NameKey.ToLower().Contains(term));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
        #endregion
    }
}