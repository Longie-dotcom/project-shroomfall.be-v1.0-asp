using Domain.Definition.EntityDomain.Component;

namespace Application.Interfaces.Cache.EntityDomain.Component
{
    public interface ILifetimeCache
    {
        void Load(
            List<LifetimeDefinition> data);
        IEnumerable<LifetimeDefinition> GetAll();
        LifetimeDefinition? Get(
            Guid id);
        LifetimeDefinition? GetByEntity(
            string entityDefinitionId);
    }
}
