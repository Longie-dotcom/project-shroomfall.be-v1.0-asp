using Application.Interfaces.Repository.NonRelational;
using Domain.Snapshot.WorldDomain;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;
using MongoDB.Driver;

namespace Infrastructure.Repository.NonRelational
{
    public class RoomConnectionSnapshotRepository : MongoGenericRepository<RoomConnectionSnapshot>, IRoomConnectionSnapshotRepository
    {
        #region Attributes
        private readonly IMongoCollection<RoomConnectionSnapshot> collection;
        #endregion

        #region Properties
        #endregion

        public RoomConnectionSnapshotRepository(
            NonRelationalDB context) : base(context)
        {
            this.collection = context.GetCollection<RoomConnectionSnapshot>(nameof(RoomConnectionSnapshot));
        }

        #region Methods
        #endregion
    }
}