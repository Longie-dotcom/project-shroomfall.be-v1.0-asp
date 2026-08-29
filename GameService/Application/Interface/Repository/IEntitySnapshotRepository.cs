using Application.Interface.Repository.Base;
using Domain.Snapshot.EntityDomain;

namespace Application.Interface.Repository
{
    public interface IEntitySnapshotRepository : IGenericRepository<EntitySnapshot>, IRepository
    {
        Task<IEnumerable<EntitySnapshot>> GetByRoomIdAsync(
            string roomSpatialId);
        Task<IEnumerable<EntitySnapshot>> GetPlayerSnapshotByUserIdAsync(
            string userId);
        Task DeleteMissingUnownedEntitiesInRoomAsync(
            string roomSpatialId,
            IEnumerable<string> activeEntityIds);
    }
}
