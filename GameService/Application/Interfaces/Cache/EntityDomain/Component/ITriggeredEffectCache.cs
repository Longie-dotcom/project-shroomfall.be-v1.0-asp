using Domain.Definition.EntityDomain.Component;

namespace Application.Interfaces.Cache.EntityDomain.Component
{
    public interface ITriggeredEffectCache
    {
        void Load(
            List<TriggeredEffectDefinition> data);
        IEnumerable<TriggeredEffectDefinition> GetAll();
        TriggeredEffectDefinition? Get(
            Guid id);
        TriggeredEffectDefinition? GetByEntity(
            string entityDefinitionId);
    }
}
