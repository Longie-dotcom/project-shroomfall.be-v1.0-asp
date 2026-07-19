using Application.Interfaces.Repository.NonRelational;
using Domain.Abstraction;
using Domain.Snapshot.EntityDomain;
using Domain.Snapshot.EntityDomain.Component;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repository.NonRelational
{
    public class EntitySnapshotRepository : MongoGenericRepository<EntitySnapshot>, IEntitySnapshotRepository
    {
        #region Attributes
        private readonly IMongoCollection<EntitySnapshot> collection;
        #endregion

        #region Properties
        #endregion

        public EntitySnapshotRepository(
            NonRelationalDB context) : base(context)
        {
            collection = context.GetCollection<EntitySnapshot>(nameof(EntitySnapshot));
        }

        #region Methods
        public async Task<IEnumerable<EntitySnapshot>> GetByRoomIdAsync(string roomSpatialId)
        {
            var filter = Builders<EntitySnapshot>.Filter.ElemMatch(
                entity => entity.Components,
                Builders<ComponentSnapshot>.Filter.OfType<TransformSnapshot>(
                    Builders<TransformSnapshot>.Filter.Eq(c => c.RoomSpatialID, roomSpatialId)));

            return await collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<EntitySnapshot>> GetPlayerSnapshotByUserIdAsync(string userId)
        {
            Console.WriteLine($"[Mongo] Searching ownership for user {userId}");

            var filter = Builders<EntitySnapshot>.Filter.ElemMatch(
                "Components",
                Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("_t", nameof(OwnershipSnapshot)),
                    Builders<BsonDocument>.Filter.Eq("UserID", userId)));

            var result = await collection.Find(filter).ToListAsync();

            Console.WriteLine($"[Mongo] Found {result.Count} snapshots");

            foreach (var entity in result)
                Console.WriteLine($"[Mongo] {entity.ID}");

            return result;
        }
        #endregion
    }
}