using Domain.Definition.WorldDomain;

namespace Application.Interfaces.Repository.Relational
{
    public interface ITileRepository : ISQLGenericRepository<Tile>, IRelationalRepository
    {

    }
}
