using Application.Interfaces.Cache.EntityDomain.Component;
using Domain.Definition.EntityDomain.Component;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Infrastructure.Cache.EntityDomain.Component
{
    public class SpawnCache : ISpawnCache
    {
        #region Attributes
        private Dictionary<Guid, SpawnDefinition> byId = new();
        private Dictionary<string, SpawnDefinition> byEntityId = new();
        #endregion

        #region Properties
        #endregion

        public SpawnCache() { }

        #region Methods
        public void Load(
            List<SpawnDefinition> data)
        {
            byId = data.ToDictionary(x => x.ID, x => x);

            byEntityId.Clear();

            foreach (var item in data)
            {
                var key = item.EntityDefinitionID;

                if (byEntityId.TryGetValue(key, out var existing))
                    throw new InternalException(
                        InfrastructureCode.SpawnCacheCode.DuplicateSpawnComponent,
                        $"Duplicate component detected for EntityDefinitionID '{key}' in {typeof(SpawnDefinition).Name}. Existing: {existing.ID}, New: {item.ID}");

                byEntityId[key] = item;
            }
        }

        public IEnumerable<SpawnDefinition> GetAll()
        {
            return byId.Values;
        }

        public SpawnDefinition? Get(
            Guid id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }

        public SpawnDefinition? GetByEntity(
            string entityDefinitionId)
        {
            byEntityId.TryGetValue(entityDefinitionId, out var value);
            return value;
        }
        #endregion
    }
}