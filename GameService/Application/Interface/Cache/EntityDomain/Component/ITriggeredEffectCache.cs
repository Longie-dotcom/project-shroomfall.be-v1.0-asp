using Contract.DTO.Definition.EntityDomain.Component;

namespace Application.Interface.Cache.EntityDomain.Component
{
    public interface ITriggeredEffectCache
    {
        void Load(
            List<TriggeredEffectDefinitionDTO> data);
        IEnumerable<TriggeredEffectDefinitionDTO> GetAll();
        TriggeredEffectDefinitionDTO? Get(
            Guid id);
        TriggeredEffectDefinitionDTO? GetByEntity(
            string entityDefinitionId);
    }
}
