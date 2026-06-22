using Domain.Definition.EntityDomain.Component;

namespace Application.Interfaces.Cache.EntityDomain.Component
{
    public interface IPortalCache
    {
        void Load(
            List<PortalDefinition> data);
        IEnumerable<PortalDefinition> GetAll();
        PortalDefinition? Get(
            Guid id);
        PortalDefinition? GetByEntity(
            string entityDefinitionId);
    }
}
