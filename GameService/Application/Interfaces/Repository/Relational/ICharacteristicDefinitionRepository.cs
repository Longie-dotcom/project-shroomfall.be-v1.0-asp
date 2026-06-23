using Application.Interfaces.Repository.Base;
using Domain.Definition.EntityDomain.Component;

namespace Application.Interfaces.Repository.Relational
{
    public interface ICharacteristicDefinitionRepository : ISQLDefinitionRepository<CharacteristicDefinition>, IRelationalRepository
    {
        Task SaveAttributeValuesAsync(
            IEnumerable<AttributeValue> attributeValues);
        Task SaveAttributeGrowthValuesAsync(
            IEnumerable<AttributeGrowthValue> growthValues);
        Task ReplaceAttributeValuesAsync(
            Guid characteristicId,
            IEnumerable<AttributeValue> newValues);
        Task ReplaceAttributeGrowthValuesAsync(
            Guid attributeValueId,
            IEnumerable<AttributeGrowthValue> newGrowths);
    }
}
