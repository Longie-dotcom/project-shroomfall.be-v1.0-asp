using Domain.Definition.EntityDomain;

namespace Application.Interfaces.Cache
{
    public interface IEntityRelationshipCache
    {
        void Load(
            IEnumerable<EntityRelationship> data);
        IReadOnlyCollection<EntityRelationship> GetAll();
        IEnumerable<EntityRelationship>? GetBySourceID(
            string id);
    }
}