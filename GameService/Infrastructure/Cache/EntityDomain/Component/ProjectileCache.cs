using Application.Interfaces.Cache.EntityDomain.Component;
using Domain.Definition.EntityDomain.Component;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Infrastructure.Cache.EntityDomain.Component
{
    public class ProjectileCache : IProjectileCache
    {
        #region Attributes
        private Dictionary<Guid, ProjectileDefinition> byId = new();
        private Dictionary<string, ProjectileDefinition> byEntityId = new();
        #endregion

        #region Properties
        #endregion

        public ProjectileCache() { }

        #region Methods
        public void Load(
            List<ProjectileDefinition> data)
        {
            byId = data.ToDictionary(x => x.ID, x => x);

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

        public IEnumerable<ProjectileDefinition> GetAll()
        {
            return byId.Values;
        }

        public ProjectileDefinition? Get(
            Guid id)
        {
            byId.TryGetValue(id, out var value);
            return value;
        }

        public ProjectileDefinition? GetByEntity(
            string entityDefinitionId)
        {
            byEntityId.TryGetValue(entityDefinitionId, out var value);
            return value;
        }
        #endregion
    }
}