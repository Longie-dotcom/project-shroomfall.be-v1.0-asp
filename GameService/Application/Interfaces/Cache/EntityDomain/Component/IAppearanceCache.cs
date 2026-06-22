using Domain.Definition.EntityDomain.Component;

namespace Application.Interfaces.Cache.EntityDomain.Component
{
    public interface IAppearanceCache
    {
        void Load(
            List<AppearanceDefinition> data);
        IEnumerable<AppearanceDefinition> GetAll();
        AppearanceDefinition? Get(
            Guid id);
        AppearanceDefinition? GetByEntity(
            string entityDefinitionId);
    }
}
