using Application.Interfaces.Cache;
using Domain.Runtime.WorldDomain.Spatial;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;


namespace Application.Services.WorldService.Factory
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

            var cells = roomDef.Cells;
            if (cells == null || !cells.Any())
                throw new InternalException(
                    ApplicationCode.RoomSpatialFactoryCode.RehydrateRoomWithoutCells,
                    $"Room definition has no cells: {snapshot.DefinitionID}");

            return new RoomSpatial(
                id: snapshot.ID,
                definitionId: roomDef.ID,
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

            var cells = roomDef.Cells;
            if (cells == null || !cells.Any())
                throw new InternalException(
                    ApplicationCode.RoomSpatialFactoryCode.CreateRoomWithoutCells,
                    $"Room definition has no cells: {definitionId}");

            return new RoomSpatial(
                id: instanceId,
                definitionId: roomDef.ID,
                ownerId: ownerId);
        }
        #endregion
    }
}