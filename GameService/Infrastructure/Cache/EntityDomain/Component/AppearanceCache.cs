using Application.Interfaces.Cache.EntityDomain.Component;
using Domain.Definition.EntityDomain.Component;
using Domain.DomainException;
using ResponseCode;

namespace Infrastructure.Cache.EntityDomain.Component
{
    public class AppearanceCache : IAppearanceCache
    {
        #region Attributes
        private Dictionary<Guid, AppearanceDefinition> byId = new();
        private Dictionary<string, AppearanceDefinition> byEntityId = new();
        #endregion

        #region Properties
        #endregion

        public AppearanceCache() { }

        #region Methods
        public void Load(
            List<AppearanceDefinition> data)
        {
            byId = data.ToDictionary(x => x.ID, x => x);

            byEntityId.Clear();

            foreach (var item in data)
            {
                var key = item.EntityDefinitionID;

                if (byEntityId.TryGetValue(key, out var existing))
                    throw new InternalException(
                        InfrastructureCode.AppearanceCacheCode.DuplicateAppearanceComponent,
                        $"Duplicate component detected for EntityDefinitionID '{key}' in {typeof(AppearanceDefinition).Name}. Existing: {existing.ID}, New: {item.ID}");

                byEntityId[key] = item;
            }
        }

        public IEnumerable<AppearanceDefinition> GetAll()
        {
            return byId.Values;
        }

        public AppearanceDefinition? Get(
            Guid id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }

        public AppearanceDefinition? GetByEntity(
            string entityDefinitionId)
        {
            byEntityId.TryGetValue(entityDefinitionId, out var value);
            return value;
        }
        #endregion
    }
}