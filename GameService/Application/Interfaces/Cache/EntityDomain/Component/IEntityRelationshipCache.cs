using Domain.Definition.EntityDomain.Component;

namespace Application.Interfaces.Cache.EntityDomain.Component
{
    public interface IEntityRelationshipCache
    {
        void Load(
            List<EntityRelationshipDefinition> data);
        IEnumerable<EntityRelationshipDefinition> GetAll();
        EntityRelationshipDefinition? Get(
            Guid id);
        EntityRelationshipDefinition? GetByEntity(
            string entityDefinitionId);
    }
}
