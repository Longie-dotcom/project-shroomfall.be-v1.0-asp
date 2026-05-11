using Application.Helper;
using Application.Interfaces.Factory;
using Application.Interfaces.Repository.NonRelational;
using Application.Services.Abstraction.WorldService;
using AutoMapper;
using Domain.Document.WorldDomain;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.WorldDomain.World;

namespace Application.Services.WorldService
{
    public class SaveService : ISaveService
    {
        #region Attributes
        private readonly INonRelationalUoW nonRelational;
        private readonly IMapper mapper;
        private readonly IEntityDocumentFactory entityDocumentFactory;
        #endregion

        #region Properties
        #endregion

        public SaveService(
            INonRelationalUoW nonRelational,
            IMapper mapper,
            IEntityDocumentFactory entityDocumentFactory)
        {
            this.nonRelational = nonRelational;
            this.mapper = mapper;
            this.entityDocumentFactory = entityDocumentFactory;
        }

        #region Methods
        public async Task<RoomSnapshot?> LoadRoomSnapshotAsync(
            string roomId)
        {
            var roomRepo = nonRelational.GetRepository<IRoomDocumentRepository>();
            var entityRepo = nonRelational.GetRepository<IEntityDocumentRepository>();

            var roomDoc = await roomRepo.GetByIdAsync(roomId);
            if (roomDoc == null)
                return null;

            var room = mapper.Map<RoomSpatial>(roomDoc);

            var entities = await entityRepo.GetByRoomIdAsync(roomId);

            var runtimeEntities = entities
                .Select(e => EntityDocumentMapper.ToRuntime(e, entityDocumentFactory))
                .ToList();

            return new RoomSnapshot
            {
                Room = room,
                Entities = runtimeEntities
            };
        }

        public async Task<PlayerInstance?> LoadPlayerAsync(
            string playerInstanceId)
        {
            var entityRepo = nonRelational.GetRepository<IEntityDocumentRepository>();

            var doc = await entityRepo.GetByIdAsync(playerInstanceId);

            if (doc == null)
                return null;

            return EntityDocumentMapper.ToRuntime(doc, entityDocumentFactory) as PlayerInstance;
        }

        public async Task SaveRoomAsync(
            RoomSnapshot snapshot)
        {
            var roomRepo = nonRelational.GetRepository<IRoomDocumentRepository>();
            var entityRepo = nonRelational.GetRepository<IEntityDocumentRepository>();

            // Save room metadata
            var roomDoc = mapper.Map<RoomDocument>(snapshot.Room);
            await roomRepo.UpdateAsync(roomDoc);

            // Save entities inside room
            foreach (var entity in snapshot.Entities)
            {
                var doc = EntityDocumentMapper.ToDocument(entity, mapper);
                if (doc == null)
                    continue;

                await entityRepo.UpdateAsync(doc);
            }
        }

        public async Task SaveEntityAsync(
            EntityInstance entity)
        {
            var entityRepo = nonRelational.GetRepository<IEntityDocumentRepository>();

            var doc = EntityDocumentMapper.ToDocument(entity, mapper);
            if (doc == null)
                return;

            await entityRepo.UpdateAsync(doc);
        }

        public async Task SaveWorldAsync(WorldContext context)
        {
            // SAFETY CHECK: no unresolved expansions allowed
            if (context.PendingRooms != null && context.PendingRooms.Count > 0)
            {
                throw new InvalidOperationException(
                    "Cannot persist WorldContext with pending room initializations.");
            }

            var roomRepo = nonRelational.GetRepository<IRoomDocumentRepository>();
            var entityRepo = nonRelational.GetRepository<IEntityDocumentRepository>();

            // Save rooms
            foreach (var room in context.Rooms)
            {
                var roomDoc = mapper.Map<RoomDocument>(room);
                await roomRepo.UpdateAsync(roomDoc);
            }

            // Save entities
            foreach (var entity in context.Entities)
            {
                var doc = EntityDocumentMapper.ToDocument(entity, mapper);
                if (doc == null)
                    continue;

                await entityRepo.UpdateAsync(doc);
            }
        }
        #endregion
    }
}