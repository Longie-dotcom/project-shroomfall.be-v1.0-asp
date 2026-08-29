using Application.Interface.Cache.EntityDomain.Component;
using Contract.DTO.Definition.EntityDomain.Component;
using Domain.DomainException;
using ResponseCode;

namespace Infrastructure.Cache.EntityDomain.Component
{
    public class CollisionCache : ICollisionCache
    {
        #region Attributes
        private Dictionary<Guid, CollisionDefinitionDTO> byId = new();
        private Dictionary<string, CollisionDefinitionDTO> byEntityId = new();
        #endregion

        #region Properties
        #endregion

        public CollisionCache() { }

        #region Methods
        public void Load(
            List<CollisionDefinitionDTO> data)
        {
            byId = data.ToDictionary(x => x.ID!.Value, x => x);

            byEntityId.Clear();

            foreach (var item in data)
            {
                var key = item.EntityDefinitionID;

                if (byEntityId.TryGetValue(key, out var existing))
                    throw new InternalException(
                        InfrastructureCode.CollisionCacheCode.DuplicateCollisionComponent,
                        $"Duplicate component detected for EntityDefinitionID '{key}' in {typeof(CollisionCache).Name}. Existing: {existing.ID}, New: {item.ID}");

                byEntityId[key] = item;
            }
        }

        public IEnumerable<CollisionDefinitionDTO> GetAll()
        {
            return byId.Values;
        }

        public CollisionDefinitionDTO? Get(
            Guid id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }

        public CollisionDefinitionDTO? GetByEntity(
            string entityDefinitionId)
        {
            byEntityId.TryGetValue(entityDefinitionId, out var value);
            return value;
        }
        #endregion
    }
}