using Application.Interfaces.Repository.NonRelational;
using Domain.Document.EntityDomain;
using Infrastructure.Persistence;
using MongoDB.Driver;

namespace Infrastructure.Repository.NonRelational
{
    public class EntityDocumentRepository : MongoGenericRepository<EntityDocument>, IEntityDocumentRepository, INonRelationalRepository
    {
        #region Attributes
        private readonly NonRelationalDB context;
        private readonly IMongoCollection<EntityDocument> collection;
        #endregion

        #region Properties
        #endregion

        public EntityDocumentRepository(
            NonRelationalDB context) : base(
                context)
        {
            this.context = context;
            this.collection = context.GetCollection<EntityDocument>(nameof(EntityDocument));
        }

        #region Methods
        public async Task<IReadOnlyList<EntityDocument>> GetByRoomIdAsync(
            string roomId)
        {
            var filter = Builders<EntityDocument>
                .Filter
                .Eq(x => x.RoomSpatialID, roomId);

            return await collection
                .Find(filter)
                .ToListAsync();
        }
        #endregion
    }
}