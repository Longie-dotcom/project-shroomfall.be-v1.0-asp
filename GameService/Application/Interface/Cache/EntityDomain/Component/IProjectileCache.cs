using Contract.DTO.Definition.EntityDomain.Component;

namespace Application.Interface.Cache.EntityDomain.Component
{
    public interface IProjectileCache
    {
        void Load(
            List<ProjectileDefinitionDTO> data);
        IEnumerable<ProjectileDefinitionDTO> GetAll();
        ProjectileDefinitionDTO? Get(
            Guid id);
        ProjectileDefinitionDTO? GetByEntity(
            string entityDefinitionId);
    }
}
