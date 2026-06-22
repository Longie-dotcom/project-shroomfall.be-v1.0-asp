using Domain.Definition.EntityDomain.Component;

namespace Application.Interfaces.Cache.EntityDomain.Component
{
    public interface IInteractableCache
    {
        void Load(
            List<InteractableDefinition> data);
        IEnumerable<InteractableDefinition> GetAll();
        InteractableDefinition? Get(
            Guid id);
        InteractableDefinition? GetByEntity(
            string entityDefinitionId);
    }
}
