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
                Builders<ComponentSnapshot>.Filter.And(
                    Builders<ComponentSnapshot>.Filter.Eq(c => c.Type, nameof(TransformSnapshot)),
                    Builders<ComponentSnapshot>.Filter.Eq("RoomSpatialID", roomSpatialId)));

            return await collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<EntitySnapshot>> GetPlayerDocumentsByUserIdAsync(string userId)
        {
            var filter = Builders<EntitySnapshot>.Filter.ElemMatch(
                entity => entity.Components,
                Builders<ComponentSnapshot>.Filter.And(
                    Builders<ComponentSnapshot>.Filter.Eq(c => c.Type, nameof(OwnershipSnapshot)),
                    Builders<ComponentSnapshot>.Filter.Eq("UserID", userId)));

            return await collection.Find(filter).ToListAsync();
        }
        #endregion
    }
}