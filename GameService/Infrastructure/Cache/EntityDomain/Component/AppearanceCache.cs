using Application.Interface.Cache.EntityDomain.Component;
using Contract.DTO.Definition.EntityDomain.Component;
using Domain.DomainException;
using ResponseCode;

namespace Infrastructure.Cache.EntityDomain.Component
{
    public class AppearanceCache : IAppearanceCache
    {
        #region Attributes
        private Dictionary<Guid, AppearanceDefinitionDTO> byId = new();
        private Dictionary<string, AppearanceDefinitionDTO> byEntityId = new();
        #endregion

        #region Properties
        #endregion

        public AppearanceCache() { }

        #region Methods
        public void Load(
            List<AppearanceDefinitionDTO> data)
        {
            byId = data.ToDictionary(x => x.ID!.Value, x => x);

            byEntityId.Clear();

            foreach (var item in data)
            {
                var key = item.EntityDefinitionID;

                if (byEntityId.TryGetValue(key, out var existing))
                    throw new InternalException(
                        InfrastructureCode.AppearanceCacheCode.DuplicateAppearanceComponent,
                        $"Duplicate component detected for EntityDefinitionID '{key}' in {typeof(AppearanceCache).Name}. Existing: {existing.ID}, New: {item.ID}");

                byEntityId[key] = item;
            }
        }

        public IEnumerable<AppearanceDefinitionDTO> GetAll()
        {
            return byId.Values;
        }

        public AppearanceDefinitionDTO? Get(
            Guid id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }

        public AppearanceDefinitionDTO? GetByEntity(
            string entityDefinitionId)
        {
            byEntityId.TryGetValue(entityDefinitionId, out var value);
            return value;
        }
        #endregion
    }
}