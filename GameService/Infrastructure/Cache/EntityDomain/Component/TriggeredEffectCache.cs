using Application.Interfaces.Cache.EntityDomain.Component;
using Domain.Definition.EntityDomain.Component;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Infrastructure.Cache.EntityDomain.Component
{
    public class TriggeredEffectCache : ITriggeredEffectCache
    {
        #region Attributes
        private Dictionary<Guid, TriggeredEffectDefinition> byId = new();
        private Dictionary<string, TriggeredEffectDefinition> byEntityId = new();
        #endregion

        #region Properties
        #endregion

        public TriggeredEffectCache() { }

        #region Methods
        public void Load(
            List<TriggeredEffectDefinition> data)
        {
            byId = data.ToDictionary(x => x.ID, x => x);

            byEntityId.Clear();

            foreach (var item in data)
            {
                var key = item.EntityDefinitionID;

                if (byEntityId.TryGetValue(key, out var existing))
                    throw new InternalException(
                        InfrastructureCode.TriggeredEffectCacheCode.DuplicateTriggeredEffectComponent,
                        $"Duplicate component detected for EntityDefinitionID '{key}' in {nameof(TriggeredEffectDefinition)}. Existing: {existing.ID}, New: {item.ID}");

                byEntityId[key] = item;
            }
        }

        public IEnumerable<TriggeredEffectDefinition> GetAll()
        {
            return byId.Values;
        }

        public TriggeredEffectDefinition? Get(
            Guid id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }

        public TriggeredEffectDefinition? GetByEntity(
            string entityDefinitionId)
        {
            byEntityId.TryGetValue(entityDefinitionId, out var value);
            return value;
        }
        #endregion
    }
}