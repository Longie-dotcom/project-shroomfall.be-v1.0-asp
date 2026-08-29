using Contract.DTO.Definition.EntityDomain.Component;

namespace Application.Interface.Cache.EntityDomain.Component
{
    public interface ILifetimeCache
    {
        void Load(
            List<LifetimeDefinitionDTO> data);
        IEnumerable<LifetimeDefinitionDTO> GetAll();
        LifetimeDefinitionDTO? Get(
            Guid id);
        LifetimeDefinitionDTO? GetByEntity(
            string entityDefinitionId);
    }
}
