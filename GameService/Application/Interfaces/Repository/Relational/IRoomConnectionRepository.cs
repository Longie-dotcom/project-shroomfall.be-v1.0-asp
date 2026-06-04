using Domain.Definition.WorldDomain;

namespace Application.Interfaces.Repository.Relational
{
    public interface IRoomConnectionRepository : ISQLGenericRepository<RoomConnection>, IRelationalRepository
    {

    }
}