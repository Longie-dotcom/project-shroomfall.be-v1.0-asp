using Application.Interfaces.Factory;
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
        private readonly IRoomSpatialFactory roomSpatialFactory;
        #endregion

        #region Properties
        #endregion

        public RoomPersistence(
            IMapper mapper,
            INonRelationalUoW nonRelational,
            IRoomSpatialFactory roomSpatialFactory)
        {
            this.mapper = mapper;
            this.nonRelational = nonRelational;
            this.roomSpatialFactory = roomSpatialFactory;
        }

        #region Methods
        public async Task<RoomSpatial?> LoadAsync(
            string roomSpatialId)
        {
            var roomRepo = nonRelational.GetRepository<IRoomDocumentRepository>();

            var doc = await roomRepo.GetByIdAsync(roomSpatialId);
            if (doc == null)
                return null;

            return roomSpatialFactory.CreateFromDocument(doc);
        }

        public async Task SaveAsync(
            RoomSpatial room)
        {
            var roomRepo = nonRelational.GetRepository<IRoomDocumentRepository>();

            var doc = mapper.Map<RoomDocument>(room);

            await roomRepo.UpdateAsync(doc);
        }
        #endregion
    }
}