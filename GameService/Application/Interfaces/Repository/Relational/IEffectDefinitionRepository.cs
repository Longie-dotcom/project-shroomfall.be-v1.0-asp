using Application.Interfaces.Repository.Base;
using Contract.Enum.MetaDomain.Effect;
using Domain.Definition.MetaDomain;

namespace Application.Interfaces.Repository.Relational
{
    public interface IEffectDefinitionRepository : ISQLGenericRepository<EffectDefinition>, IRelationalRepository
    {
        Task<(IEnumerable<EffectDefinition> Items, int TotalCount)> GetPagedDefinitionsAsync(
            string? searchTerm,
            EffectType? type,
            AttributeType? attributeType,
            AttributeType? sourceType,
            int pageNumber,
            int pageSize);
    }
}
