using Application.Interfaces.Cache.EntityDomain.Component;
using Domain.Definition.EntityDomain.Component;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Infrastructure.Cache.EntityDomain.Component
{
    public class InteractableCache : IInteractableCache
    {
        #region Attributes
        private Dictionary<Guid, InteractableDefinition> byId = new();
        private Dictionary<string, InteractableDefinition> byEntityId = new();
        #endregion

        #region Properties
        #endregion

        public InteractableCache() { }

        #region Methods
        public void Load(
            List<InteractableDefinition> data)
        {
            byId = data.ToDictionary(x => x.ID, x => x);

            byEntityId.Clear();

            foreach (var item in data)
            {
                var key = item.EntityDefinitionID;

                if (byEntityId.TryGetValue(key, out var existing))
                    throw new InternalException(
                        InfrastructureCode.InteractableCacheCode.DuplicateInteractableComponent,
                        $"Duplicate component detected for EntityDefinitionID '{key}' in {typeof(InteractableDefinition).Name}. Existing: {existing.ID}, New: {item.ID}");

                byEntityId[key] = item;
            }
        }

        public IEnumerable<InteractableDefinition> GetAll()
        {
            return byId.Values;
        }

        public InteractableDefinition? Get(
            Guid id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }

        public InteractableDefinition? GetByEntity(
            string entityDefinitionId)
        {
            byEntityId.TryGetValue(entityDefinitionId, out var value);
            return value;
        }
        #endregion
    }
}