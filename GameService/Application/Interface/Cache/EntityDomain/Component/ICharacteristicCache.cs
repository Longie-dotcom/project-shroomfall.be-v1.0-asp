using Contract.DTO.Definition.EntityDomain.Component;
using Contract.Enum.MetaDomain.Effect;

namespace Application.Interface.Cache.EntityDomain.Component
{
    public interface ICharacteristicCache
    {
        void Load(
            List<CharacteristicDefinitionDTO> data);
        IEnumerable<CharacteristicDefinitionDTO> GetAll();
        CharacteristicDefinitionDTO? Get(
            Guid id);
        CharacteristicDefinitionDTO? GetByEntity(
            string entityDefinitionId);
        (AttributeValueDTO Attribute, AttributeGrowthValueDTO Growth)? GetAttributeValue(
            Guid characteristicId, 
            int level, 
            AttributeType type);
    }
}
