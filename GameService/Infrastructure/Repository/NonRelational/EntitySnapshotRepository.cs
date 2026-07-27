using Application.Interfaces.Repository.NonRelational;
using Domain.Abstraction;
using Domain.Snapshot.EntityDomain;
using Domain.Snapshot.EntityDomain.Component;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;
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
        public async Task<IEnumerable<EntitySnapshot>> GetByRoomIdAsync(
            string roomSpatialId)
        {
            var filter = Builders<EntitySnapshot>.Filter.ElemMatch(
                entity => entity.Components,
                Builders<ComponentSnapshot>.Filter.OfType<TransformSnapshot>(
                    Builders<TransformSnapshot>.Filter.Eq(c => c.RoomSpatialID, roomSpatialId)));

            return await collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<EntitySnapshot>> GetPlayerSnapshotByUserIdAsync(
            string userId)
        {
            var filter = Builders<EntitySnapshot>.Filter.ElemMatch(
                entity => entity.Components,
                Builders<ComponentSnapshot>.Filter.OfType<OwnershipSnapshot>(
                    Builders<OwnershipSnapshot>.Filter.Eq(c => c.UserID, userId)));

            return await collection.Find(filter).ToListAsync();
        }

        public async Task DeleteMissingUnownedEntitiesInRoomAsync(
            string roomSpatialId,
            IEnumerable<string> activeEntityIds)
        {
            var filterBuilder = Builders<EntitySnapshot>.Filter;

            // 1. Target entities located in this specific room
            var roomFilter = filterBuilder.ElemMatch(
                entity => entity.Components,
                Builders<ComponentSnapshot>.Filter.OfType<TransformSnapshot>(
                    c => c.RoomSpatialID == roomSpatialId));

            // 2. Exclude entities that are currently active in runtime
            var missingFilter = filterBuilder.Not(
                filterBuilder.In(entity => entity.ID, activeEntityIds));

            // 3. EXCLUDE PLAYER/OWNED ENTITIES (Only target entities that have NO OwnershipSnapshot)
            var hasNoOwnershipFilter = filterBuilder.Not(
                filterBuilder.ElemMatch(
                    entity => entity.Components,
                    Builders<ComponentSnapshot>.Filter.OfType<OwnershipSnapshot>()));

            // Combine all conditions
            var finalFilter = filterBuilder.And(roomFilter, missingFilter, hasNoOwnershipFilter);

            await collection.DeleteManyAsync(finalFilter);
        }
        #endregion
    }
}