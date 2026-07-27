using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.NonRelational;
using Application.Services.WorldService.Factory;
using AutoMapper;
using Domain.Abstraction;
using Domain.Runtime.EntityDomain;
using Domain.Snapshot.EntityDomain;

namespace Application.Services.WorldService.Persistence
{
    public class EntityPersistence
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly INonRelationalUoW nonRelationalUoW;
        private readonly EntityInstanceFactory entityInstanceFactory;
        #endregion

        #region Properties
        #endregion

        public EntityPersistence(
            IMapper mapper,
            INonRelationalUoW nonRelationalUoW,
            EntityInstanceFactory entityInstanceFactory)
        {
            this.mapper = mapper;
            this.nonRelationalUoW = nonRelationalUoW;
            this.entityInstanceFactory = entityInstanceFactory;
        }

        #region Methods
        public async Task<EntityInstance?> LoadEntityAsync(
            string entityId)
        {
            // Resolve repository
            var entitySnapshotRepo = nonRelationalUoW.GetRepository<IEntitySnapshotRepository>();

            // Fetch the raw data bag (Snapshot) from Mongo
            var snapshot = await entitySnapshotRepo.GetByIdAsync(entityId);
            if (snapshot == null) 
                return null;

            // Convert Snapshot -> Domain Object
            var entity = entityInstanceFactory.Rehydrate(snapshot);

            return entity;
        }

        public async Task<List<EntityInstance>> LoadByRoomAsync(
            string roomSpatialId)
        {
            var entitySnapshotRepo = nonRelationalUoW.GetRepository<IEntitySnapshotRepository>();
            var snapshots = await entitySnapshotRepo.GetByRoomIdAsync(roomSpatialId);
            return snapshots.Select(x => entityInstanceFactory.Rehydrate(x)).ToList();
        }

        public async Task SaveManyAsync(
            IEnumerable<EntityInstance> entities)
        {
            var entitySnapshotRepo = nonRelationalUoW.GetRepository<IEntitySnapshotRepository>();
            var snapshots = entities.Select(e => mapper.Map<EntitySnapshot>(e));
            await entitySnapshotRepo.UpdateManyAsync(snapshots);
        }

        public async Task DeleteMissingEntitiesInRoomAsync(
            string roomSpatialId,
            IEnumerable<string> activeEntityIds)
        {
            var entitySnapshotRepo = nonRelationalUoW.GetRepository<IEntitySnapshotRepository>();
            await entitySnapshotRepo.DeleteMissingEntitiesInRoomAsync(roomSpatialId, activeEntityIds);
        }
        #endregion
    }
}