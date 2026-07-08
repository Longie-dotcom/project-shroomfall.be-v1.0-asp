using Application.Interfaces.Cache.EntityDomain.Component;
using Domain.Definition.EntityDomain.Component;
using Domain.DomainException;
using ResponseCode;

namespace Infrastructure.Cache.EntityDomain.Component
{
    public class AICache : IAICache
    {
        #region Attributes
        private Dictionary<Guid, AIDefinition> byId = new();
        private Dictionary<string, AIDefinition> byEntityId = new();
        #endregion

        #region Properties
        #endregion

        public AICache() { }

        #region Methods
        public void Load(
            List<AIDefinition> data)
        {
            byId = data.ToDictionary(x => x.ID, x => x);

            byEntityId.Clear();

            foreach (var item in data)
            {
                var key = item.EntityDefinitionID;

                if (byEntityId.TryGetValue(key, out var existing))
                    throw new InternalException(
                        InfrastructureCode.AICacheCode.DuplicateAIComponent,
                        $"Duplicate component detected for EntityDefinitionID '{key}' in {typeof(AICache).Name}. Existing: {existing.ID}, New: {item.ID}");

                byEntityId[key] = item;
            }
        }

        public IEnumerable<AIDefinition> GetAll()
        {
            return byId.Values;
        }

        public AIDefinition? Get(
            Guid id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }

        public AIDefinition? GetByEntity(
            string entityDefinitionId)
        {
            byEntityId.TryGetValue(entityDefinitionId, out var value);
            return value;
        }
        #endregion
    }
}