namespace Application.Feature.Admin.Command
{
    public class FetchRoomInstanceCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; set; }
        public string RoomSpatialID { get; set; }
        #endregion

        public FetchRoomInstanceCommand(
            string userId, 
            string roomSpatialId)
        {
            UserID = userId;
            RoomSpatialID = roomSpatialId;
        }

        #region Methods
        #endregion
    }
}