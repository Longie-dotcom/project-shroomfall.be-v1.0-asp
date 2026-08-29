using Application.Interface.Cache.EntityDomain.Component;
using Contract.DTO.Definition.EntityDomain.Component;
using Domain.DomainException;
using ResponseCode;

namespace Infrastructure.Cache.EntityDomain.Component
{
    public class AICache : IAICache
    {
        #region Attributes
        private Dictionary<Guid, AIDefinitionDTO> byId = new();
        private Dictionary<string, AIDefinitionDTO> byEntityId = new();
        #endregion

        #region Properties
        #endregion

        public AICache() { }

        #region Methods
        public void Load(
            List<AIDefinitionDTO> data)
        {
            byId = data.ToDictionary(x => x.ID!.Value, x => x);

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

        public IEnumerable<AIDefinitionDTO> GetAll()
        {
            return byId.Values;
        }

        public AIDefinitionDTO? Get(
            Guid id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }

        public AIDefinitionDTO? GetByEntity(
            string entityDefinitionId)
        {
            byEntityId.TryGetValue(entityDefinitionId, out var value);
            return value;
        }
        #endregion
    }
}