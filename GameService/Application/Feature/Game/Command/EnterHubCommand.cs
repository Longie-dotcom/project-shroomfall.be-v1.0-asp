namespace Application.Feature.Game.Command
{
    public class EnterHubCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public string HubRoomSpatialID { get; }
        #endregion

        public EnterHubCommand(
            string userId,
            string hubRoomSpatialId)
        {
            UserID = userId;
            HubRoomSpatialID = hubRoomSpatialId;
        }

        #region Methods
        #endregion
    }
}