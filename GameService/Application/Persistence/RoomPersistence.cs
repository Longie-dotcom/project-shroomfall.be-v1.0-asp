using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.NonRelational;
using Application.Services.WorldService.Factory;
using AutoMapper;
using Domain.Runtime.WorldDomain.Spatial;
using Domain.Snapshot.WorldDomain;

namespace Application.Persistence
{
    public class RoomPersistence
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly INonRelationalUoW nonRelationalUoW;
        private readonly RoomSpatialFactory roomSpatialFactory;
        #endregion

        #region Properties
        #endregion

        public RoomPersistence(
            IMapper mapper,
            INonRelationalUoW nonRelationalUoW,
            RoomSpatialFactory roomSpatialFactory)
        {
            this.mapper = mapper;
            this.nonRelationalUoW = nonRelationalUoW;
            this.roomSpatialFactory = roomSpatialFactory;
        }

        #region Methods
        public async Task<RoomSpatial?> LoadAsync(
            string roomSpatialId)
        {
            var roomRepo = nonRelationalUoW.GetRepository<IRoomSnapshotRepository>();

            var snapshot = await roomRepo.GetByIdAsync(roomSpatialId);
            if (snapshot == null)
                return null;

            return roomSpatialFactory.Rehydrate(snapshot);
        }

        public async Task SaveAsync(
            RoomSpatial room)
        {
            var roomRepo = nonRelationalUoW.GetRepository<IRoomSnapshotRepository>();

            var doc = mapper.Map<RoomSnapshot>(room);

            await roomRepo.UpdateAsync(doc);
        }
        #endregion
    }
}