using Domain.DomainException;
using Domain.Shared;

namespace Domain.Runtime.WorldDomain
{
    public class RoomConnectionInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; }
        public string DefinitionID { get; }
        public string SourceRoomSpatialID { get; }
        public string SourceEntityInstanceID { get; }
        public string? DestinationRoomSpatialID { get; private set; }
        public string? DestinationEntityInstanceID { get; private set; }
        #endregion

        public RoomConnectionInstance(
            string id,
            string definitionId,
            string sourceRoomSpatialId,
            string sourceEntityInstanceId,
            string? destinationRoomSpatialId,
            string? destinationEntityInstanceId)
        {
            ID = id;
            DefinitionID = definitionId;
            SourceRoomSpatialID = sourceRoomSpatialId;
            SourceEntityInstanceID = sourceEntityInstanceId;
            DestinationRoomSpatialID = destinationRoomSpatialId;
            DestinationEntityInstanceID = destinationEntityInstanceId;
        }

        #region Methods
        public bool IsInstantiated()
        {
            return !string.IsNullOrWhiteSpace(DestinationRoomSpatialID);
        }

        public void BindDestination(
            string roomSpatialId,
            string entityInstanceId)
        {
            if (string.IsNullOrWhiteSpace(roomSpatialId))
                throw new BadRequest(ResponseCode.RoomConnectionInstance_InvalidDestinationRoomSpatialId);

            if (string.IsNullOrWhiteSpace(entityInstanceId))
                throw new BadRequest(ResponseCode.RoomConnectionInstance_InvalidDestinationEntityInstanceId);

            DestinationRoomSpatialID = roomSpatialId;
            DestinationEntityInstanceID = entityInstanceId;
        }
        #endregion
    }
}