using Domain.Definition.EntityDomain;

namespace Application.Interfaces.Repository.Relational
{
    public interface IEntityRelationshipRepository : ISQLGenericRepository<EntityRelationship>, IRelationalRepository
    {

    }
}
