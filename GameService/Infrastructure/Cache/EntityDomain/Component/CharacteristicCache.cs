using Application.Interface.Cache.EntityDomain.Component;
using Contract.DTO.Definition.EntityDomain.Component;
using Contract.Enum.MetaDomain.Effect;
using Domain.DomainException;
using ResponseCode;

namespace Infrastructure.Cache.EntityDomain.Component
{
    public class CharacteristicCache : ICharacteristicCache
    {
        #region Attributes
        private Dictionary<Guid, CharacteristicDefinitionDTO> byId = new();
        private Dictionary<string, CharacteristicDefinitionDTO> byEntityId = new();
        private Dictionary<(Guid CharacteristicId, AttributeType Type, int Level), (AttributeValueDTO Attribute, AttributeGrowthValueDTO Growth)> attributeLookup = new();
        #endregion

        #region Properties
        #endregion

        public CharacteristicCache() { }

        #region Methods
        public void Load(
            List<CharacteristicDefinitionDTO> data)
        {
            byId = data.ToDictionary(x => x.ID!.Value, x => x);

            byEntityId.Clear();

            foreach (var item in data)
            {
                var key = item.EntityDefinitionID;

                if (byEntityId.TryGetValue(key, out var existing))
                    throw new InternalException(
                        InfrastructureCode.CharacteristicCacheCode.DuplicateCharacteristicComponent,
                        $"Duplicate component detected for EntityDefinitionID '{key}' in {typeof(CharacteristicCache).Name}. Existing: {existing.ID}, New: {item.ID}");

                byEntityId[key] = item;

                foreach (var attr in item.AttributeValues)
                {
                    foreach (var growth in attr.AttributeGrowthValues)
                    {
                        attributeLookup[(item.ID!.Value, attr.Type, growth.Level)] = (attr, growth);
                    }
                }
            }
        }

        public IEnumerable<CharacteristicDefinitionDTO> GetAll()
        {
            return byId.Values;
        }

        public CharacteristicDefinitionDTO? Get(
            Guid id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }

        public CharacteristicDefinitionDTO? GetByEntity(
            string entityDefinitionId)
        {
            byEntityId.TryGetValue(entityDefinitionId, out var value);
            return value;
        }

        public (AttributeValueDTO Attribute, AttributeGrowthValueDTO Growth)? GetAttributeValue(
            Guid characteristicId,
            int level,
            AttributeType type)
        {
            return attributeLookup.TryGetValue((characteristicId, type, level), out var value)
                ? value
                : null;
        }
        #endregion
    }
}