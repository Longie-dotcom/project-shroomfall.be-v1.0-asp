using Domain.Document.WorldDomain;

namespace Application.Interfaces.Repository.NonRelational
{
    public interface IRoomDocumentRepository : IMongoGenericRepository<RoomDocument>, INonRelationalRepository
    {
        Task<RoomDocument?> GetByOwnerIdAsync(
            string ownerId);
    }
}
