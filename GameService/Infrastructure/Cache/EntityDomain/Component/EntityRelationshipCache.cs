using Application.Interfaces.Cache.EntityDomain.Component;
using Domain.Definition.EntityDomain.Component;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Infrastructure.Cache.EntityDomain.Component
{
    public class EntityRelationshipCache : IEntityRelationshipCache
    {
        #region Attributes
        private Dictionary<Guid, EntityRelationshipDefinition> byId = new();
        private Dictionary<string, EntityRelationshipDefinition> byEntityId = new();
        #endregion

        #region Properties
        #endregion

        public EntityRelationshipCache() { }

        #region Methods
        public void Load(
            List<EntityRelationshipDefinition> data)
        {
            byId = data.ToDictionary(x => x.ID, x => x);

            byEntityId.Clear();

            foreach (var item in data)
            {
                var key = item.EntityDefinitionID;

                if (byEntityId.TryGetValue(key, out var existing))
                    throw new InternalException(
                        InfrastructureCode.EntityRelationshipCacheCode.DuplicateEntityRelationshipComponent,
                        $"Duplicate component detected for EntityDefinitionID '{key}' in {typeof(EntityRelationshipDefinition).Name}. Existing: {existing.ID}, New: {item.ID}");

                byEntityId[key] = item;
            }
        }

        public IEnumerable<EntityRelationshipDefinition> GetAll()
        {
            return byId.Values;
        }

        public EntityRelationshipDefinition? Get(
            Guid id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }

        public EntityRelationshipDefinition? GetByEntity(
            string entityDefinitionId)
        {
            byEntityId.TryGetValue(entityDefinitionId, out var value);
            return value;
        }
        #endregion
    }
}