using Domain.Definition.EntityDomain.Component;

namespace Application.Interfaces.Cache.EntityDomain.Component
{
    public interface IAICache
    {
        void Load(
            List<AIDefinition> data);
        IEnumerable<AIDefinition> GetAll();
        AIDefinition? Get(
            Guid id);
        AIDefinition? GetByEntity(
            string entityDefinitionId);
    }
}
