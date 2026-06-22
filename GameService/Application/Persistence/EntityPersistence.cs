using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.NonRelational;
using Application.Services.WorldService.Factory;
using AutoMapper;
using Domain.Runtime.EntityDomain;
using Domain.Snapshot.EntityDomain;

namespace Application.Persistence
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

            if (snapshot == null) return null;

            // Convert Snapshot -> Domain Object
            var entity = entityInstanceFactory.Rehydrate(snapshot);

            return entity;
        }

        public async Task<List<EntityInstance>> LoadByRoomAsync(
            string roomSpatialId)
        {
            // Resolve repository
            var entitySnapshotRepo = nonRelationalUoW.GetRepository<IEntitySnapshotRepository>();

            var snapshots = await entitySnapshotRepo.GetByRoomIdAsync(roomSpatialId);

            return snapshots.Select(x => entityInstanceFactory.Rehydrate(x)).ToList();
        }

        public async Task SaveManyAsync(
            IEnumerable<EntityInstance> entities)
        {
            // Resolve repository
            var entitySnapshotRepo = nonRelationalUoW.GetRepository<IEntitySnapshotRepository>();

            // Project the collection of Domain Entities to the collection of Snapshots
            var snapshots = entities.Select(e => mapper.Map<EntitySnapshot>(e));

            // Save all snapshots in one bulk operation
            await entitySnapshotRepo.UpdateManyAsync(snapshots);
        }
        #endregion
    }
}