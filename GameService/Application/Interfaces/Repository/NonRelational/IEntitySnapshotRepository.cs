using Application.Interfaces.Repository.Base;
using Domain.Snapshot.EntityDomain;

namespace Application.Interfaces.Repository.NonRelational
{
    public interface IEntitySnapshotRepository : IMongoGenericRepository<EntitySnapshot>, INonRelationalRepository
    {
        Task<IEnumerable<EntitySnapshot>> GetByRoomIdAsync(
            string roomSpatialId);
        Task<IEnumerable<EntitySnapshot>> GetPlayerSnapshotByUserIdAsync(
            string userId);
        Task DeleteMissingEntitiesInRoomAsync(
            string roomSpatialId,
            IEnumerable<string> activeEntityIds);
    }
}
