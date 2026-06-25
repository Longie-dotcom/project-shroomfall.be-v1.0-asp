using Contract.Enum.MetaDomain.Effect;
using Domain.Definition.EntityDomain.Component;

namespace Application.Interfaces.Cache.EntityDomain.Component
{
    public interface ICharacteristicCache
    {
        void Load(
            List<CharacteristicDefinition> data);
        IEnumerable<CharacteristicDefinition> GetAll();
        CharacteristicDefinition? Get(
            Guid id);
        CharacteristicDefinition? GetByEntity(
            string entityDefinitionId);
        (AttributeValue Attribute, AttributeGrowthValue Growth)? GetAttributeValue(
            Guid characteristicId, 
            int level, 
            AttributeType type);
    }
}
