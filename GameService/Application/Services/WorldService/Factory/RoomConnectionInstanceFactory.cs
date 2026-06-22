using Application.Interfaces.Cache;
using Domain.Runtime.WorldDomain.Topology;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;
using Domain.Snapshot.WorldDomain;

namespace Application.Services.WorldService.Factory
{
    public class RoomConnectionInstanceFactory
    {
        #region Attributes
        private readonly ICacheProvider cacheProvider;
        #endregion

        #region Properties
        #endregion

        public RoomConnectionInstanceFactory(
            ICacheProvider cacheProvider)
        {
            this.cacheProvider = cacheProvider;
        }

        #region Methods
        public RoomConnectionInstance Rehydrate(
            RoomConnectionSnapshot snapshot)
        {
            var def = cacheProvider.RoomConnection.Get(snapshot.DefinitionID);
            if (def == null)
                throw new InternalException(
                    ApplicationCode.RoomConnectionInstanceFactoryCode.RehydrateDefinitionNotFound,
                    $"Room connection definition not found: {snapshot.DefinitionID}");

            if (string.IsNullOrWhiteSpace(snapshot.SourceRoomSpatialID) ||
                string.IsNullOrWhiteSpace(snapshot.SourceEntityInstanceID))
                throw new InternalException(
                    ApplicationCode.RoomConnectionInstanceFactoryCode.RehydrateInvalidInstanceData,
                    "Room connection instance has invalid runtime linkage data");

            return new RoomConnectionInstance(
                id: Guid.NewGuid().ToString(),
                definitionId: def.ID,
                sourceRoomSpatialId: snapshot.SourceRoomSpatialID,
                sourceEntityInstanceId: snapshot.SourceEntityInstanceID,
                destinationRoomSpatialId: snapshot.DestinationRoomSpatialID,
                destinationEntityInstanceId: snapshot.DestinationEntityInstanceID,
                null
            );
        }

        public RoomConnectionInstance Create(
            string definitionId,
            string sourceRoomSpatialId,
            string sourceEntityInstanceId,
            string? destinationRoomSpatialId,
            string? destinationEntityInstanceId)
        {
            var def = cacheProvider.RoomConnection.Get(definitionId);
            if (def == null)
                throw new InternalException(
                    ApplicationCode.RoomConnectionInstanceFactoryCode.CreateDefinitionNotFound,
                    $"Room connection definition not found: {definitionId}");

            if (string.IsNullOrWhiteSpace(sourceRoomSpatialId) ||
                string.IsNullOrWhiteSpace(sourceEntityInstanceId))
                throw new InternalException(
                    ApplicationCode.RoomConnectionInstanceFactoryCode.CreateInvalidInstanceData,
                    "Room connection instance has invalid runtime linkage data");

            return new RoomConnectionInstance(
                id: Guid.NewGuid().ToString(),
                definitionId: def.ID,
                sourceRoomSpatialId: sourceRoomSpatialId,
                sourceEntityInstanceId: sourceEntityInstanceId,
                destinationRoomSpatialId: destinationRoomSpatialId,
                destinationEntityInstanceId: destinationEntityInstanceId,
                null
            );
        }
        #endregion
    }
}