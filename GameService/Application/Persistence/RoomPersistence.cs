using Application.Interfaces.Repository.NonRelational;
using AutoMapper;
using Domain.Document.WorldDomain;
using Domain.Runtime.WorldDomain;

namespace Application.Persistence
{
    public class RoomPersistence
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly INonRelationalUoW nonRelational;
        #endregion

        #region Properties
        #endregion

        public RoomPersistence(
            IMapper mapper,
            INonRelationalUoW nonRelational)
        {
            this.mapper = mapper;
            this.nonRelational = nonRelational;
        }

        #region Methods
        public async Task<RoomSpatial?> LoadAsync(
            string roomSpatialId)
        {
            var roomRepo = nonRelational.GetRepository<IRoomDocumentRepository>();

            var doc = await roomRepo.GetByIdAsync(roomSpatialId);
            if (doc == null)
                return null;

            return mapper.Map<RoomSpatial>(doc);
        }

        public async Task SaveAsync(
            RoomSpatial room)
        {
            var roomRepo = nonRelational.GetRepository<IRoomDocumentRepository>();

            var doc = mapper.Map<RoomDocument>(room);

            await roomRepo.UpdateAsync(doc);
        }

        public async Task SaveManyAsync(
            IEnumerable<RoomSpatial> rooms)
        {
            var roomRepo = nonRelational.GetRepository<IRoomDocumentRepository>();

            var docs = rooms.Select(mapper.Map<RoomDocument>).ToList();

            await roomRepo.UpdateManyAsync(docs);
        }
        #endregion
    }
}