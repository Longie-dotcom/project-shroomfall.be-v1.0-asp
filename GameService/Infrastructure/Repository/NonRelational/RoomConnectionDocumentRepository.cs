using Application.Interfaces.Repository.NonRelational;
using Domain.Document.WorldDomain;
using Infrastructure.Persistence;
using MongoDB.Driver;

namespace Infrastructure.Repository.NonRelational
{
    public class RoomConnectionDocumentRepository : MongoGenericRepository<RoomConnectionDocument>, IRoomConnectionDocumentRepository, INonRelationalRepository
    {
        #region Attributes
        private readonly NonRelationalDB context;
        private readonly IMongoCollection<RoomConnectionDocument> collection;
        #endregion

        #region Properties
        #endregion

        public RoomConnectionDocumentRepository(
            NonRelationalDB context) : base(
                context)
        {
            this.context = context;
            this.collection = context.GetCollection<RoomConnectionDocument>(nameof(RoomConnectionDocument));
        }

        #region Methods
        #endregion
    }
}