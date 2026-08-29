using Application.Interface.Repository;
using Application.Interface.Repository.Base;
using Application.Service.WorldService.Factory;
using AutoMapper;
using Domain.Runtime.WorldDomain.Spatial;

namespace Application.Service.WorldService.Persistence
{
    public class RoomPersistence
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly IUnitOfWork nonRelationalUoW;
        private readonly RoomSpatialFactory roomSpatialFactory;
        #endregion

        #region Properties
        #endregion

        public RoomPersistence(
            IMapper mapper,
            IUnitOfWork nonRelationalUoW,
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
            var snapshot = mapper.Map<Domain.Snapshot.WorldDomain.RoomSnapshot>(room);
            await roomRepo.UpdateAsync(snapshot);
        }
        #endregion
    }
}