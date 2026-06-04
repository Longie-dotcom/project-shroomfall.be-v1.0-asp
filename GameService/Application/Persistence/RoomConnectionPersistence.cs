using Application.Interfaces.Repository.NonRelational;
using AutoMapper;
using Domain.Document.WorldDomain;
using Domain.Runtime.WorldDomain;

namespace Application.Persistence
{
    public class RoomConnectionPersistence
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly INonRelationalUoW nonRelational;
        #endregion

        #region Properties
        #endregion

        public RoomConnectionPersistence(
            IMapper mapper,
            INonRelationalUoW nonRelational)
        {
            this.mapper = mapper;
            this.nonRelational = nonRelational;
        }

        #region Methods
        public async Task SaveAsync(
            RoomConnectionInstance connection)
        {
            var repo = nonRelational.GetRepository<IRoomConnectionDocumentRepository>();

            var doc = mapper.Map<RoomConnectionDocument>(connection);

            await repo.UpdateAsync(doc);
        }
        #endregion
    }
}