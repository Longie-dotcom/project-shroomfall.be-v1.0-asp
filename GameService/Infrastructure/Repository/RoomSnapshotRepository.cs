using Application.Interface.Repository;
using Domain.Snapshot.WorldDomain;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;
using MongoDB.Driver;

namespace Infrastructure.Repository
{
    public class RoomSnapshotRepository : GenericRepository<RoomSnapshot>, IRoomSnapshotRepository
    {
        #region Attributes
        private readonly IMongoCollection<RoomSnapshot> collection;
        #endregion

        #region Properties
        #endregion

        public RoomSnapshotRepository(
            GameDBContext context) : base(context)
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