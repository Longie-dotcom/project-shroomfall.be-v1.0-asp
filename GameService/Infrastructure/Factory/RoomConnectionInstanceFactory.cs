using Application.Interfaces.Cache;
using Application.Interfaces.Factory;
using Domain.Document.WorldDomain;
using Domain.DomainException;
using Domain.Runtime.WorldDomain;
using Domain.Shared;

namespace Infrastructure.Factory
{
    public class RoomConnectionInstanceFactory : IRoomConnectionInstanceFactory
    {
        #region Attributes
        private readonly IRoomConnectionCache roomConnectionCache;
        #endregion

        #region Properties
        #endregion

        public RoomConnectionInstanceFactory(
            IRoomConnectionCache roomConnectionCache)
        {
            this.roomConnectionCache = roomConnectionCache;
        }

        #region Methods
        public RoomConnectionInstance Create(
            string definitionId,
            string sourceRoomSpatialId,
            string sourceEntityInstanceId,
            string? destinationRoomSpatialId,
            string? destinationEntityInstanceId)
        {
            var def = roomConnectionCache.Get(definitionId);
            if (def == null)
                throw new InternalException(
                    ResponseCode.RoomConnectionInstanceFactory_DefinitionNotFound,
                    $"Room connection definition with ID: {definitionId} not found in cache");

            if (string.IsNullOrWhiteSpace(sourceRoomSpatialId) ||
                string.IsNullOrWhiteSpace(sourceEntityInstanceId))
                throw new InternalException(
                    ResponseCode.RoomConnectionInstanceFactory_InvalidInstanceData,
                    "Room connection instance has invalid runtime linkage data");

            return new RoomConnectionInstance(
                id: Guid.NewGuid().ToString(),
                definitionId: def.ID,
                sourceRoomSpatialId: sourceRoomSpatialId,
                sourceEntityInstanceId: sourceEntityInstanceId,
                destinationRoomSpatialId: destinationRoomSpatialId,
                destinationEntityInstanceId: destinationEntityInstanceId
            );
        }

        public RoomConnectionInstance CreateFromDocument(
            RoomConnectionDocument doc)
        {
            if (doc == null)
                throw new InternalException(
                    ResponseCode.RoomConnectionInstanceFactory_DocumentNull,
                    "Room connection document is null");

            var def = roomConnectionCache.Get(doc.DefinitionID);
            if (def == null)
                throw new InternalException(
                    ResponseCode.RoomConnectionInstanceFactory_DefinitionFromDocumentNotFound,
                    $"Room connection definition with ID: {doc.DefinitionID} not found in cache");

            return new RoomConnectionInstance(
                id: doc.ID,
                definitionId: doc.DefinitionID,
                sourceRoomSpatialId: doc.SourceRoomSpatialID,
                sourceEntityInstanceId: doc.SourceEntityInstanceID,
                destinationRoomSpatialId: doc.DestinationRoomSpatialID,
                destinationEntityInstanceId: doc.DestinationEntityInstanceID
            );
        }
        #endregion
    }
}