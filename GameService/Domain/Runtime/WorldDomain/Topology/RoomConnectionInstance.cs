namespace Domain.Runtime.WorldDomain.Topology
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
        public string? ReverseConnectionID { get; private set; }
        #endregion

        public RoomConnectionInstance(
            string id,
            string definitionId,
            string sourceRoomSpatialId,
            string sourceEntityInstanceId,
            string? destinationRoomSpatialId,
            string? destinationEntityInstanceId,
            string? reverseConnectionId)
        {
            ID = id;
            DefinitionID = definitionId;
            SourceRoomSpatialID = sourceRoomSpatialId;
            SourceEntityInstanceID = sourceEntityInstanceId;
            DestinationRoomSpatialID = destinationRoomSpatialId;
            DestinationEntityInstanceID = destinationEntityInstanceId;
            ReverseConnectionID = reverseConnectionId;
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
            DestinationRoomSpatialID = roomSpatialId;
            DestinationEntityInstanceID = entityInstanceId;
        }

        public void SetReverseConnection(
            string connectionId)
        {
            ReverseConnectionID = connectionId;
        }
        #endregion
    }
}