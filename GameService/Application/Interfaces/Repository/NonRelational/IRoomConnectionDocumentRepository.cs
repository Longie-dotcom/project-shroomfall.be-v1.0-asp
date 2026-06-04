using Domain.Document.WorldDomain;

namespace Application.Interfaces.Repository.NonRelational
{
    public interface IRoomConnectionDocumentRepository : IMongoGenericRepository<RoomConnectionDocument>, INonRelationalRepository
    {

    }
}
