using Application.Context;
using Application.Interfaces.Cache;
using Application.Services.WorldService.Factory;
using Domain.Definition.WorldDomain;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.WorldDomain.Spatial;
using Domain.Runtime.WorldDomain.Topology;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Application.Services.WorldService
{
    public class TopologyService
    {
        #region Attributes
        private readonly WorldContext worldContext;
        private readonly ICacheProvider cacheProvider;
        private readonly RoomConnectionInstanceFactory roomConnectionInstanceFactory;
        private readonly InitializationService initializationService;
        #endregion

        #region Properties
        #endregion

        public TopologyService(
            WorldContext worldContext,
            ICacheProvider cacheProvider,
            RoomConnectionInstanceFactory roomConnectionInstanceFactory,
            InitializationService initializationService)
        {
            this.worldContext = worldContext;
            this.cacheProvider = cacheProvider;
            this.roomConnectionInstanceFactory = roomConnectionInstanceFactory;
            this.initializationService = initializationService;
        }

        #region Methods
        public async Task<(RoomConnectionInstance ConnectionForward, RoomConnectionInstance? ConnectionReverse, RoomSnapshot? NewRoomSnapshot, bool IsNewRoom)>
            ResolveOrCreateConnection(string entityInstanceId)
        {
            var entity = RequireEntity(entityInstanceId);

            // Reuse existing dynamic runtime connections if already active
            var existing = GetExistingConnection(entityInstanceId);
            if (existing != null)
            {
                return (existing, null, null, false);
            }

            // Resolve the predesigned structural rule blueprint
            var (sourceRoomInstance, connectionDefinition) = ResolveConnectionDefinition(entity);

            // Spawns the flat layout instance block
            RoomSnapshot destinationSnapshot = CreateDestinationRoom(connectionDefinition);

            // Locate the anchor doorway entity matching the target layout
            var destinationEntityInstance = ResolveDestinationEntity(destinationSnapshot, connectionDefinition.DestinationEntityID);

            // Create forward tracking with accurate signature parameters
            var connectionForward = CreateConnection(
                definitionId: connectionDefinition.ID,
                sourceRoomSpatialId: sourceRoomInstance.ID,
                sourceEntityInstanceId: entity.ID,
                destinationRoomSpatialId: destinationSnapshot.Room.ID,
                destinationEntityInstanceId: destinationEntityInstance.ID
            );

            RoomConnectionInstance? connectionReverse = null;

            // Query the return blueprint rule treating the new room as the SOURCE
            var reverseDefinition = cacheProvider.RoomConnection.GetBySource(
                destinationSnapshot.Room.DefinitionID,
                destinationEntityInstance.DefinitionID
            );

            if (reverseDefinition != null)
            {
                // Create the reciprocal reverse shortcut connection
                connectionReverse = CreateConnection(
                    definitionId: reverseDefinition.ID,
                    sourceRoomSpatialId: destinationSnapshot.Room.ID,
                    sourceEntityInstanceId: destinationEntityInstance.ID,
                    destinationRoomSpatialId: sourceRoomInstance.ID,
                    destinationEntityInstanceId: entity.ID
                );

                // Tie both ends of the gateway together permanently
                connectionForward.SetReverseConnection(connectionReverse.ID);
                connectionReverse.SetReverseConnection(connectionForward.ID);
            }

            return (connectionForward, connectionReverse, destinationSnapshot, true);
        }

        private EntityInstance RequireEntity(string entityInstanceId)
        {
            var entity = worldContext.GetEntity(entityInstanceId);
            if (entity == null)
                throw new InternalException(
                    ApplicationCode.TopologyServiceCode.EntityNotFound,
                    $"Topology resolution aborted. Entity target '{entityInstanceId}' could not be resolved from active context.");

            return entity;
        }

        private RoomConnectionInstance? GetExistingConnection(string entityInstanceId)
        {
            var connection = worldContext.GetConnectionByEntityInstanceID(entityInstanceId);
            if (connection == null || !connection.IsInstantiated())
                return null;

            return connection;
        }

        private (RoomSpatial Room, RoomConnection Definition) ResolveConnectionDefinition(EntityInstance entity)
        {
            var transform = entity.GetComponent<TransformInstance>();
            if (transform == null)
                throw new InternalException(
                    ApplicationCode.TopologyServiceCode.TransformComponentMissing,
                    $"Topology resolution aborted. Entity '{entity.ID}' (Def: '{entity.DefinitionID}') is missing required structural Transform component.");

            var room = worldContext.GetRoom(transform.RoomSpatialID);
            if (room == null)
                throw new InternalException(
                    ApplicationCode.TopologyServiceCode.CurrentRoomNotFound,
                    $"Topology resolution aborted. Current active room boundary '{transform.RoomSpatialID}' holding Entity '{entity.ID}' was not found.");

            var definition = cacheProvider.RoomConnection.GetBySource(room.DefinitionID, entity.DefinitionID);
            if (definition == null)
                throw new InternalException(
                    ApplicationCode.TopologyServiceCode.ConnectionDefinitionMissing,
                    $"Topology resolution aborted. No spatial connection blueprint is defined matching Source Room Type '{room.DefinitionID}' and Trigger Entity Type '{entity.DefinitionID}'.");

            return (room, definition);
        }

        private RoomSnapshot CreateDestinationRoom(RoomConnection connectionDefinition)
        {
            var roomSpatialId = Guid.NewGuid().ToString();

            return initializationService.InitializeRoom(
                connectionDefinition.DestinationRoomID,
                roomSpatialId,
                null,
                null).room;
        }

        private EntityInstance ResolveDestinationEntity(RoomSnapshot snapshot, string destinationEntityDefinitionId)
        {
            var entity = snapshot.Entities
                .FirstOrDefault(x => x.DefinitionID == destinationEntityDefinitionId);

            if (entity == null)
                throw new InternalException(
                    ApplicationCode.TopologyServiceCode.DestinationEntityMissing,
                    $"Topology binding aborted. The initialized target Room '{snapshot.Room.ID}' (Def: '{snapshot.Room.DefinitionID}') failed to generate expected anchor Entity Blueprint '{destinationEntityDefinitionId}'.");

            return entity;
        }

        private RoomConnectionInstance CreateConnection(
            string definitionId,
            string sourceRoomSpatialId,
            string sourceEntityInstanceId,
            string destinationRoomSpatialId,
            string destinationEntityInstanceId)
        {
            return roomConnectionInstanceFactory.Create(
                definitionId: definitionId,
                sourceRoomSpatialId: sourceRoomSpatialId,
                sourceEntityInstanceId: sourceEntityInstanceId,
                destinationRoomSpatialId: destinationRoomSpatialId,
                destinationEntityInstanceId: destinationEntityInstanceId);
        }
        #endregion
    }
}