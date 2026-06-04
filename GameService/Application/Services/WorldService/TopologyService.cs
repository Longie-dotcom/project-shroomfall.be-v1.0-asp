using Application.Context;
using Application.Interfaces.Cache;
using Application.Interfaces.Factory;
using Domain.Definition.WorldDomain;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.WorldDomain;
using Domain.Shared;

namespace Application.Services.WorldService
{
    public class TopologyService
    {
        #region Attributes
        private readonly WorldContext worldContext;
        private readonly IRoomConnectionCache roomConnectionCache;
        private readonly IRoomConnectionInstanceFactory roomConnectionInstanceFactory;
        private readonly InitializationService initializationService;
        #endregion

        #region Properties
        #endregion

        public TopologyService(
            WorldContext worldContext,
            IRoomConnectionCache roomConnectionCache,
            IRoomConnectionInstanceFactory roomConnectionInstanceFactory,
            InitializationService initializationService)
        {
            this.worldContext = worldContext;
            this.roomConnectionCache = roomConnectionCache;
            this.roomConnectionInstanceFactory = roomConnectionInstanceFactory;
            this.initializationService = initializationService;
        }

        #region Methods
        public async Task<(RoomConnectionInstance Connection, RoomSnapshot? NewRoomSnapshot, bool IsNewRoom)>
            ResolveOrCreateConnection(string entityInstanceId)
        {
            // Resolve source entity
            var entity = RequireEntity(entityInstanceId);

            // Reuse existing connection
            var existing = GetExistingConnection(entityInstanceId);
            if (existing != null)
            {
                return (existing, null, false);
            }

            // Resolve topology definition
            var (room, connectionDefinition) = ResolveConnectionDefinition(entity);

            // Create destination room (NEW ROOM CASE)
            var snapshot = CreateDestinationRoom(connectionDefinition);

            // Resolve destination entity
            var destinationEntity = ResolveDestinationEntity(
                snapshot,
                connectionDefinition.DestinationEntityID);

            // Create runtime connection
            var connection = CreateConnection(
                room.ID,
                entity.ID,
                snapshot.Room.ID,
                destinationEntity.ID);

            return (connection, snapshot, true);
        }

        private EntityInstance RequireEntity(
            string entityInstanceId)
        {
            var entity = worldContext.GetEntity<EntityInstance>(entityInstanceId);
            if (entity == null)
                throw new InternalException(
                    ResponseCode.TopologyService_EntityNotFound,
                    $"Entity not found: {entityInstanceId}");

            return entity;
        }

        private RoomConnectionInstance? GetExistingConnection(
            string entityInstanceId)
        {
            var connection = worldContext.GetConnectionByEntityInstanceID(entityInstanceId);
            if (connection == null || !connection.IsInstantiated())
                return null;

            return connection;
        }

        private (RoomSpatial Room, RoomConnection Definition) ResolveConnectionDefinition(
            EntityInstance entity)
        {
            var room = worldContext.GetRoom(entity.RoomSpatialID);
            if (room == null)
                throw new InternalException(
                    ResponseCode.TopologyService_RoomNotFound,
                    $"Room not found: {entity.RoomSpatialID}");

            var definition = roomConnectionCache.GetBySource(room.DefinitionID, entity.DefinitionID);
            if (definition == null)
                throw new InternalException(
                    ResponseCode.TopologyService_NoConnectionDefinition,
                    "No connection definition for this entity");

            return (room, definition);
        }

        private RoomSnapshot CreateDestinationRoom(
            RoomConnection connectionDefinition)
        {
            var roomSpatialId = Guid.NewGuid().ToString();

            return initializationService.InitializeRoom(
                connectionDefinition.DestinationRoomID,
                roomSpatialId,
                null);
        }

        private EntityInstance ResolveDestinationEntity(
            RoomSnapshot snapshot,
            string destinationEntityDefinitionId)
        {
            var entity = snapshot.Entities
                .FirstOrDefault(x => x.DefinitionID == destinationEntityDefinitionId);

            if (entity == null)
                throw new InternalException(
                    ResponseCode.TopologyService_DestinationEntityMissing,
                    "Destination entity not found in initialized room");

            return entity;
        }

        private RoomConnectionInstance CreateConnection(
            string sourceRoomSpatialId,
            string sourceEntityInstanceId,
            string destinationRoomSpatialId,
            string destinationEntityInstanceId)
        {
            return roomConnectionInstanceFactory.Create(
                definitionId: Guid.NewGuid().ToString(),
                sourceRoomSpatialId: sourceRoomSpatialId,
                sourceEntityInstanceId: sourceEntityInstanceId,
                destinationRoomSpatialId: destinationRoomSpatialId,
                destinationEntityInstanceId: destinationEntityInstanceId);
        }
        #endregion
    }
}