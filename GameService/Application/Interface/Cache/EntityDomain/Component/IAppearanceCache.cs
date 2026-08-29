using Contract.DTO.Definition.EntityDomain.Component;

namespace Application.Interface.Cache.EntityDomain.Component
{
    public interface IAppearanceCache
    {
        void Load(
            List<AppearanceDefinitionDTO> data);
        IEnumerable<AppearanceDefinitionDTO> GetAll();
        AppearanceDefinitionDTO? Get(
            Guid id);
        AppearanceDefinitionDTO? GetByEntity(
            string entityDefinitionId);
    }
}
