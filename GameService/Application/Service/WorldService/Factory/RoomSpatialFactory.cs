using Application.Interface.Cache;
using Domain.DomainException;
using Domain.Runtime.WorldDomain.Spatial;
using ResponseCode;

namespace Application.Service.WorldService.Factory
{
    public class RoomSpatialFactory
    {
        #region Attributes
        private readonly ICacheProvider cacheProvider;
        #endregion

        #region Properties
        #endregion

        public RoomSpatialFactory(
            ICacheProvider cacheProvider)
        {
            this.cacheProvider = cacheProvider;
        }

        #region Methods
        public RoomSpatial Rehydrate(
            Domain.Snapshot.WorldDomain.RoomSnapshot snapshot)
        {
            var roomDef = cacheProvider.Room.Get(snapshot.DefinitionID);
            if (roomDef == null)
                throw new InternalException(
                    ApplicationCode.RoomSpatialFactoryCode.RehydrateDefinitionNotFound,
                    $"Room definition not found: {snapshot.DefinitionID}");

            return new RoomSpatial(
                id: snapshot.ID,
                definitionId: roomDef.Id,
                ownerId: snapshot.OwnerID);
        }

        public RoomSpatial Create(
            string definitionId,
            string instanceId,
            string? ownerId)
        {
            var roomDef = cacheProvider.Room.Get(definitionId);
            if (roomDef == null)
                throw new InternalException(
                    ApplicationCode.RoomSpatialFactoryCode.CreateDefinitionNotFound,
                    $"Room definition not found: {definitionId}");

            return new RoomSpatial(
                id: instanceId,
                definitionId: roomDef.Id,
                ownerId: ownerId);
        }
        #endregion
    }
}