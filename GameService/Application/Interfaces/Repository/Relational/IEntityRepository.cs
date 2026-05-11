using Domain.Definition.EntityDomain;

namespace Application.Interfaces.Repository.Relational
{
    public interface IEntityRepository : ISQLGenericRepository<Entity>, IRelationalRepository
    {

    }
}
