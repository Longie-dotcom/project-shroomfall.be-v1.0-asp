using Domain.DomainException;
using Domain.Shared;

namespace Domain.Definition.WorldDomain
{
    public class RoomConnection
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; }
        public string SourceRoomID { get; private set; }
        public string SourceEntityID { get; private set; }
        public string DestinationRoomID { get; private set; }
        public string DestinationEntityID { get; private set; }
        #endregion

        protected RoomConnection()
        {

        }

        public RoomConnection(
            string id,
            string sourceRoomId,
            string sourceEntityId,
            string destinationRoomId,
            string destinationEntityId)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadRequest(ResponseCode.RoomConnection_InvalidId);

            if (string.IsNullOrWhiteSpace(sourceRoomId))
                throw new BadRequest(ResponseCode.RoomConnection_InvalidSourceRoomId);

            if (string.IsNullOrWhiteSpace(sourceEntityId))
                throw new BadRequest(ResponseCode.RoomConnection_InvalidSourceEntityId);

            if (string.IsNullOrWhiteSpace(destinationRoomId))
                throw new BadRequest(ResponseCode.RoomConnection_InvalidDestinationRoomId);

            if (string.IsNullOrWhiteSpace(destinationEntityId))
                throw new BadRequest(ResponseCode.RoomConnection_InvalidDestinationEntityId);

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