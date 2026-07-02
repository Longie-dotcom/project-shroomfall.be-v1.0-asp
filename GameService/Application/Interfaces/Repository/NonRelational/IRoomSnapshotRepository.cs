using Application.Interfaces.Repository.Base;
using Domain.Snapshot.WorldDomain;

namespace Application.Interfaces.Repository.NonRelational
{
    public interface IRoomSnapshotRepository : IMongoGenericRepository<RoomSnapshot>, INonRelationalRepository
    {
        Task<bool> ExistsAsync(
            string roomSpatialId);
    }
}
