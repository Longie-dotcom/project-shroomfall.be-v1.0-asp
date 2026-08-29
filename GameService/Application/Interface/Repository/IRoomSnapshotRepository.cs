using Application.Interface.Repository.Base;
using Domain.Snapshot.WorldDomain;

namespace Application.Interface.Repository
{
    public interface IRoomSnapshotRepository : IGenericRepository<RoomSnapshot>, IRepository
    {
        Task<bool> ExistsAsync(
            string roomSpatialId);
    }
}
