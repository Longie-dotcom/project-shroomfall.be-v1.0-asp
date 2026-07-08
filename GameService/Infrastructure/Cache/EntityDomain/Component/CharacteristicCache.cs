using Application.Interfaces.Cache.EntityDomain.Component;
using Contract.Enum.MetaDomain.Effect;
using Domain.Definition.EntityDomain.Component;
using Domain.DomainException;
using ResponseCode;

namespace Infrastructure.Cache.EntityDomain.Component
{
    public class CharacteristicCache : ICharacteristicCache
    {
        #region Attributes
        private Dictionary<Guid, CharacteristicDefinition> byId = new();
        private Dictionary<string, CharacteristicDefinition> byEntityId = new();
        private Dictionary<(Guid CharacteristicId, AttributeType Type, int Level), (AttributeValue Attribute, AttributeGrowthValue Growth)> attributeLookup = new();
        #endregion

        #region Properties
        #endregion

        public CharacteristicCache() { }

        #region Methods
        public void Load(
            List<CharacteristicDefinition> data)
        {
            byId = data.ToDictionary(x => x.ID, x => x);

            byEntityId.Clear();

            foreach (var item in data)
            {
                var key = item.EntityDefinitionID;

                if (byEntityId.TryGetValue(key, out var existing))
                    throw new InternalException(
                        InfrastructureCode.CharacteristicCacheCode.DuplicateCharacteristicComponent,
                        $"Duplicate component detected for EntityDefinitionID '{key}' in {typeof(CharacteristicDefinition).Name}. Existing: {existing.ID}, New: {item.ID}");

                byEntityId[key] = item;

                foreach (var attr in item.AttributeValues)
                {
                    foreach (var growth in attr.AttributeGrowthValues)
                    {
                        attributeLookup[(item.ID, attr.Type, growth.Level)] = (attr, growth);
                    }
                }
            }
        }

        public IEnumerable<CharacteristicDefinition> GetAll()
        {
            return byId.Values;
        }

        public CharacteristicDefinition? Get(
            Guid id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }

        public CharacteristicDefinition? GetByEntity(
            string entityDefinitionId)
        {
            byEntityId.TryGetValue(entityDefinitionId, out var value);
            return value;
        }

        public (AttributeValue Attribute, AttributeGrowthValue Growth)? GetAttributeValue(
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