using Application.Interfaces.Repository.NonRelational;
using Domain.Snapshot.WorldDomain;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;
using MongoDB.Driver;

namespace Infrastructure.Repository.NonRelational
{
    public class RoomSnapshotRepository : MongoGenericRepository<RoomSnapshot>, IRoomSnapshotRepository
    {
        #region Attributes
        private readonly IMongoCollection<RoomSnapshot> collection;
        #endregion

        #region Properties
        #endregion

        public RoomSnapshotRepository(
            NonRelationalDB context) : base(context)
        {
            this.collection = context.GetCollection<RoomSnapshot>(nameof(RoomSnapshot));
        }

        #region Methods
        public async Task<bool> ExistsAsync(
            string roomSpatialId)
        {
            return await collection
                .Find(x => x.ID == roomSpatialId)
                .AnyAsync();
        }
        #endregion
    }
}