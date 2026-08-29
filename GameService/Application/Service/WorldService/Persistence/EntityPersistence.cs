using Application.Interface.Repository;
using Application.Interface.Repository.Base;
using Application.Service.WorldService.Factory;
using AutoMapper;
using Domain.Abstraction;
using Domain.Runtime.EntityDomain;
using Domain.Snapshot.EntityDomain;

namespace Application.Service.WorldService.Persistence
{
    public class EntityPersistence
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly IUnitOfWork nonRelationalUoW;
        private readonly EntityInstanceFactory entityInstanceFactory;
        #endregion

        #region Properties
        #endregion

        public EntityPersistence(
            IMapper mapper,
            IUnitOfWork nonRelationalUoW,
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

        public async Task SaveManyAsync(IEnumerable<EntityInstance> entities)
        {
            var snapshots = entities.Select(e => mapper.Map<EntitySnapshot>(e)).ToList();
            if (snapshots.Count == 0)
                return;

            var entitySnapshotRepo = nonRelationalUoW.GetRepository<IEntitySnapshotRepository>();
            await entitySnapshotRepo.UpdateManyAsync(snapshots);
        }

        public async Task DeleteMissingUnownedEntitiesInRoomAsync(
            string roomSpatialId,
            IEnumerable<string> activeEntityIds)
        {
            var entitySnapshotRepo = nonRelationalUoW.GetRepository<IEntitySnapshotRepository>();
            await entitySnapshotRepo.DeleteMissingUnownedEntitiesInRoomAsync(roomSpatialId, activeEntityIds);
        }
        #endregion
    }
}