using Domain.Document.EntityDomain;

namespace Application.Interfaces.Repository.NonRelational
{
    public interface IEntityDocumentRepository : IMongoGenericRepository<EntityDocument>, INonRelationalRepository
    {
        Task<IReadOnlyList<EntityDocument>> GetByRoomIdAsync(
            string roomSpatialId);
        Task<IReadOnlyCollection<PlayerDocument>> GetPlayerDocumentsByUserIdAsync(
            string userId);
    }
}
