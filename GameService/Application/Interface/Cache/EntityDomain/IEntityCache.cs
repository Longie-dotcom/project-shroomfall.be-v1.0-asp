using Contract.DTO.Definition.EntityDomain.Component;

namespace Application.Interface.Cache.EntityDomain
{
    public interface IEntityCache
    {
        void Load(
            List<EntityDefinitionDTO> data);
        IEnumerable<EntityDefinitionDTO> GetAll();
        EntityDefinitionDTO? Get(
            string id);
    }
}
