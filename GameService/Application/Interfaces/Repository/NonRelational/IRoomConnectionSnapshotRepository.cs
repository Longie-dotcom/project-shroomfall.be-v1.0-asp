using Application.Interfaces.Repository.Base;
using Domain.Snapshot.WorldDomain;

namespace Application.Interfaces.Repository.NonRelational
{
    public interface IRoomConnectionSnapshotRepository : IMongoGenericRepository<RoomConnectionSnapshot>, INonRelationalRepository
    {

    }
}
