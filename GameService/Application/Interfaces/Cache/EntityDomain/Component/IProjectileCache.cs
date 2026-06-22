using Domain.Definition.EntityDomain.Component;

namespace Application.Interfaces.Cache.EntityDomain.Component
{
    public interface IProjectileCache
    {
        void Load(
            List<ProjectileDefinition> data);
        IEnumerable<ProjectileDefinition> GetAll();
        ProjectileDefinition? Get(
            Guid id);
        ProjectileDefinition? GetByEntity(
            string entityDefinitionId);
    }
}
