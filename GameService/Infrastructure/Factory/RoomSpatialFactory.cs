using Application.Interfaces.Cache;
using Application.Interfaces.Factory;
using Domain.Document.WorldDomain;
using Domain.DomainException;
using Domain.Runtime.WorldDomain;
using Domain.Shared;

namespace Infrastructure.Factory
{
    public class RoomSpatialFactory : IRoomSpatialFactory
    {
        #region Attributes
        private readonly IRoomCache roomCache;
        #endregion

        #region Properties
        #endregion

        public RoomSpatialFactory(
            IRoomCache roomCache)
        {
            this.roomCache = roomCache;
        }

        #region Methods
        public RoomSpatial Create(
            string definitionId,
            string instanceId,
            string? ownerId)
        {
            var roomDef = roomCache.Get(definitionId);
            if (roomDef == null)
                throw new InternalException(
                    ResponseCode.RoomSpatialFactory_DefinitionNotFound,
                    $"Room definition with ID: {definitionId} is not found in cache");

            var cells = roomDef.Cells;
            if (cells == null || !cells.Any())
                throw new InternalException(
                    ResponseCode.RoomSpatialFactory_RoomWithoutCells,
                    $"Room definition with ID: {definitionId} has no cell");

            return new RoomSpatial(
                id: instanceId,
                definitionId: roomDef.ID,
                ownerId: ownerId);
        }

        public RoomSpatial CreateFromDocument(
            RoomDocument doc)
        {
            if (doc == null)
                throw new InternalException(
                    ResponseCode.RoomSpatialFactory_DocumentNotFound,
                    "Room document is null");

            var roomDef = roomCache.Get(doc.DefinitionID);
            if (roomDef == null)
                throw new InternalException(
                    ResponseCode.RoomSpatialFactory_DefinitionFromDocumentNotFound,
                    $"Room definition with ID: {doc.DefinitionID} is not found in cache");

            var instance = new RoomSpatial(
                id: doc.ID,
                definitionId: doc.DefinitionID,
                ownerId: doc.OwnerID
            );

            return instance;
        }
        #endregion
    }
}