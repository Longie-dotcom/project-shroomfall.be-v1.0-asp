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
            string roomSpatialId)
        {
            var filter = Builders<EntityDocument>
                .Filter
                .Eq(x => x.RoomSpatialID, roomSpatialId);

            return await collection
                .Find(filter)
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<PlayerDocument>> GetPlayerDocumentsByUserIdAsync(
            string userId)
        {
            var playerCollection =
                context.GetCollection<PlayerDocument>(
                    nameof(EntityDocument));

            var filter = Builders<PlayerDocument>
                .Filter
                .Eq(x => x.UserID, userId);

            return await playerCollection
                .Find(filter)
                .ToListAsync();
        }
        #endregion
    }
}