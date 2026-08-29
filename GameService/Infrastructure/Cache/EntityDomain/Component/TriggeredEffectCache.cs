using Application.Interface.Cache.EntityDomain.Component;
using Contract.DTO.Definition.EntityDomain.Component;
using Domain.DomainException;
using ResponseCode;

namespace Infrastructure.Cache.EntityDomain.Component
{
    public class TriggeredEffectCache : ITriggeredEffectCache
    {
        #region Attributes
        private Dictionary<Guid, TriggeredEffectDefinitionDTO> byId = new();
        private Dictionary<string, TriggeredEffectDefinitionDTO> byEntityId = new();
        #endregion

        #region Properties
        #endregion

        public TriggeredEffectCache() { }

        #region Methods
        public void Load(
            List<TriggeredEffectDefinitionDTO> data)
        {
            byId = data.ToDictionary(x => x.ID!.Value, x => x);

            byEntityId.Clear();

            foreach (var item in data)
            {
                var key = item.EntityDefinitionID;

                if (byEntityId.TryGetValue(key, out var existing))
                    throw new InternalException(
                        InfrastructureCode.TriggeredEffectCacheCode.DuplicateTriggeredEffectComponent,
                        $"Duplicate component detected for EntityDefinitionID '{key}' in {nameof(TriggeredEffectCache)}. Existing: {existing.ID}, New: {item.ID}");

                byEntityId[key] = item;
            }
        }

        public IEnumerable<TriggeredEffectDefinitionDTO> GetAll()
        {
            return byId.Values;
        }

        public TriggeredEffectDefinitionDTO? Get(
            Guid id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }

        public TriggeredEffectDefinitionDTO? GetByEntity(
            string entityDefinitionId)
        {
            byEntityId.TryGetValue(entityDefinitionId, out var value);
            return value;
        }
        #endregion
    }
}