using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.NonRelational;
using Application.Services.WorldService.Factory;
using AutoMapper;
using Domain.Runtime.WorldDomain.Topology;
using Domain.Snapshot.WorldDomain;

namespace Application.Persistence
{
    public class RoomConnectionPersistence
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly INonRelationalUoW nonRelationalUoW;
        private readonly RoomConnectionInstanceFactory roomConnectionInstanceFactory;
        #endregion

        #region Properties
        #endregion

        public RoomConnectionPersistence(
            IMapper mapper,
            INonRelationalUoW nonRelationalUoW,
            RoomConnectionInstanceFactory roomConnectionInstanceFactory)
        {
            this.mapper = mapper;
            this.nonRelationalUoW = nonRelationalUoW;
            this.roomConnectionInstanceFactory = roomConnectionInstanceFactory;
        }

        #region Methods
        public async Task<List<RoomConnectionInstance>> LoadAsync()
        {
            var roomConnectionRepo = nonRelationalUoW.GetRepository<IRoomConnectionSnapshotRepository>();

            var connections = await roomConnectionRepo.GetAllAsync();

            return connections.Select(c => roomConnectionInstanceFactory.Rehydrate(c)).ToList();
        }

        public async Task SaveAsync(
            RoomConnectionInstance connection)
        {
            var roomConnectionRepo = nonRelationalUoW.GetRepository<IRoomConnectionSnapshotRepository>();

            await roomConnectionRepo.UpdateAsync(mapper.Map<RoomConnectionSnapshot>(connection));
        }
        #endregion
    }
}