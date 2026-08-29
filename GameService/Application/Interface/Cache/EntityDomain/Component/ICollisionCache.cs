using Contract.DTO.Definition.EntityDomain.Component;

namespace Application.Interface.Cache.EntityDomain.Component
{
    public interface ICollisionCache
    {
        void Load(
            List<CollisionDefinitionDTO> data);
        IEnumerable<CollisionDefinitionDTO> GetAll();
        CollisionDefinitionDTO? Get(
            Guid id);
        CollisionDefinitionDTO? GetByEntity(
            string entityDefinitionId);
    }
}
