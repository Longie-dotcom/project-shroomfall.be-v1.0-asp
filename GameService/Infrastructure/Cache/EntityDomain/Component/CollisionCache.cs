using Application.Interfaces.Cache.EntityDomain.Component;
using Domain.Definition.EntityDomain.Component;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Infrastructure.Cache.EntityDomain.Component
{
    public class CollisionCache : ICollisionCache
    {
        #region Attributes
        private Dictionary<Guid, CollisionDefinition> byId = new();
        private Dictionary<string, CollisionDefinition> byEntityId = new();
        #endregion

        #region Properties
        #endregion

        public CollisionCache() { }

        #region Methods
        public void Load(
            List<CollisionDefinition> data)
        {
            byId = data.ToDictionary(x => x.ID, x => x);

            byEntityId.Clear();

            foreach (var item in data)
            {
                var key = item.EntityDefinitionID;

                if (byEntityId.TryGetValue(key, out var existing))
                    throw new InternalException(
                        InfrastructureCode.CollisionCacheCode.DuplicateCollisionComponent,
                        $"Duplicate component detected for EntityDefinitionID '{key}' in {typeof(CollisionDefinition).Name}. Existing: {existing.ID}, New: {item.ID}");

                byEntityId[key] = item;
            }
        }

        public IEnumerable<CollisionDefinition> GetAll()
        {
            return byId.Values;
        }

        public CollisionDefinition? Get(
            Guid id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }

        public CollisionDefinition? GetByEntity(
            string entityDefinitionId)
        {
            byEntityId.TryGetValue(entityDefinitionId, out var value);
            return value;
        }
        #endregion
    }
}