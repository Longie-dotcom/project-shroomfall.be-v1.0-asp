using Domain.Definition.EntityDomain;

namespace Domain.Definition.WorldDomain
{
    public class RoomConnection
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; } = string.Empty;
        public string SourceRoomID { get; private set; } = string.Empty;
        public string SourceEntityID { get; private set; } = string.Empty;
        public string DestinationRoomID { get; private set; } = string.Empty;
        public string DestinationEntityID { get; private set; } = string.Empty;

        public RoomDefinition SourceRoom { get; private set; }
        public EntityDefinition SourceEntity { get; private set; }
        public RoomDefinition DestinationRoom { get; private set; }
        public EntityDefinition DestinationEntity { get; private set; }
        #endregion

        protected RoomConnection() { }

        public RoomConnection(
            string id,
            string sourceRoomId,
            string sourceEntityId,
            string destinationRoomId,
            string destinationEntityId)
        {
            ID = id;
            SourceRoomID = sourceRoomId;
            SourceEntityID = sourceEntityId;
            DestinationRoomID = destinationRoomId;
            DestinationEntityID = destinationEntityId;
        }

        #region Methods
        #endregion
    }
}