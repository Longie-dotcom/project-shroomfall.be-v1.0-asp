using Application.Interfaces.Repository.NonRelational;
using Domain.Document.WorldDomain;
using Infrastructure.Persistence;
using MongoDB.Driver;

namespace Infrastructure.Repository.NonRelational
{
    public class RoomDocumentRepository : MongoGenericRepository<RoomDocument>, IRoomDocumentRepository, INonRelationalRepository
    {
        #region Attributes
        private readonly NonRelationalDB context;
        private readonly IMongoCollection<RoomDocument> collection;
        #endregion

        #region Properties
        #endregion

        public RoomDocumentRepository(
            NonRelationalDB context) : base(
                context)
        {
            this.context = context;
            this.collection = context.GetCollection<RoomDocument>(nameof(RoomDocument));
        }

        #region Methods
        public async Task<RoomDocument?> GetByOwnerIdAsync(
            string ownerId)
        {
            var filter = Builders<RoomDocument>
                .Filter
                .Eq(x => x.OwnerID, ownerId);

            return await collection
                .Find(filter)
                .FirstOrDefaultAsync();
        }
        #endregion
    }
}