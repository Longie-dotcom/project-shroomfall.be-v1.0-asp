using Application.Helper;
using Application.Interfaces.Factory;
using Application.Interfaces.Repository.NonRelational;
using AutoMapper;
using Domain.Document.EntityDomain;
using Domain.Runtime.EntityDomain;

namespace Application.Persistence
{
    public class EntityPersistence
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly INonRelationalUoW nonRelational;
        private readonly IEntityInstanceFactory entityInstanceFactory;
        #endregion

        #region Properties
        #endregion

        public EntityPersistence(
            IMapper mapper,
            INonRelationalUoW nonRelational,
            IEntityInstanceFactory entityInstanceFactory)
        {
            this.mapper = mapper;
            this.nonRelational = nonRelational;
            this.entityInstanceFactory = entityInstanceFactory;
        }

        #region Methods
        public async Task<T?> LoadAsync<T>(
            string entityInstanceId) where T : EntityInstance
        {
            // Resolve repository
            var entityRepo = nonRelational.GetRepository<IEntityDocumentRepository>();

            // Retrieve entity document object
            var doc = await entityRepo.GetByIdAsync(entityInstanceId);
            if (doc == null)
                return null;

            // Map to runtime object
            return EntityMapper.ToRuntime(doc, entityInstanceFactory) as T;
        }

        public async Task<List<EntityInstance>> LoadByRoomAsync(
            string roomSpatialId)
        {
            var entityRepo = nonRelational.GetRepository<IEntityDocumentRepository>();

            var docs = await entityRepo.GetByRoomIdAsync(roomSpatialId);

            return docs.Select(x => EntityMapper.ToRuntime(x, entityInstanceFactory)).ToList();
        }

        public async Task SaveAsync(
            EntityInstance entity)
        {
            var entityRepo = nonRelational.GetRepository<IEntityDocumentRepository>();

            var doc = EntityMapper.ToDocument(entity, mapper);
            if (doc == null)
                return;

            await entityRepo.UpdateAsync(doc);
        }

        public async Task SaveManyAsync(
            IEnumerable<EntityInstance> entities)
        {
            var entityRepo = nonRelational.GetRepository<IEntityDocumentRepository>();

            var docs = entities.Select(mapper.Map<EntityDocument>).ToList();

            await entityRepo.UpdateManyAsync(docs);
        }
        #endregion
    }
}