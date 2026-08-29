using Application.Interface.Cache.EntityDomain.Component;
using Contract.DTO.Definition.EntityDomain.Component;
using Domain.DomainException;
using ResponseCode;

namespace Infrastructure.Cache.EntityDomain.Component
{
    public class ProjectileCache : IProjectileCache
    {
        #region Attributes
        private Dictionary<Guid, ProjectileDefinitionDTO> byId = new();
        private Dictionary<string, ProjectileDefinitionDTO> byEntityId = new();
        #endregion

        #region Properties
        #endregion

        public ProjectileCache() { }

        #region Methods
        public void Load(
            List<ProjectileDefinitionDTO> data)
        {
            byId = data.ToDictionary(x => x.ID!.Value, x => x);

            byEntityId.Clear();

            foreach (var item in data)
            {
                var key = item.EntityDefinitionID;

                if (byEntityId.TryGetValue(key, out var existing))
                    throw new InternalException(
                        InfrastructureCode.ProjectileCacheCode.DuplicateProjectileComponent,
                        $"Duplicate component detected for EntityDefinitionID '{key}' in {typeof(ProjectileCache).Name}. Existing: {existing.ID}, New: {item.ID}");

                byEntityId[key] = item;
            }
        }

        public IEnumerable<ProjectileDefinitionDTO> GetAll()
        {
            return byId.Values;
        }

        public ProjectileDefinitionDTO? Get(
            Guid id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }

        public ProjectileDefinitionDTO? GetByEntity(
            string entityDefinitionId)
        {
            byEntityId.TryGetValue(entityDefinitionId, out var value);
            return value;
        }
        #endregion
    }
}