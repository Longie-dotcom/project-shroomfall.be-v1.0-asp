using Domain.Definition.EntityDomain.Component;

namespace Application.Interfaces.Cache.EntityDomain.Component
{
    public interface ICollisionCache
    {
        void Load(
            List<CollisionDefinition> data);
        IEnumerable<CollisionDefinition> GetAll();
        CollisionDefinition? Get(
            Guid id);
        CollisionDefinition? GetByEntity(
            string entityDefinitionId);
    }
}
