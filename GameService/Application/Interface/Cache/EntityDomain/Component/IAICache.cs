using Contract.DTO.Definition.EntityDomain.Component;

namespace Application.Interface.Cache.EntityDomain.Component
{
    public interface IAICache
    {
        void Load(
            List<AIDefinitionDTO> data);
        IEnumerable<AIDefinitionDTO> GetAll();
        AIDefinitionDTO? Get(
            Guid id);
        AIDefinitionDTO? GetByEntity(
            string entityDefinitionId);
    }
}
