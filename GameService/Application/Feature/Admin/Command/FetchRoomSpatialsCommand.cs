namespace Application.Feature.Admin.Command
{
    public class FetchRoomSpatialsCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; set; }
        #endregion

        public FetchRoomSpatialsCommand(
            string userId)
        {
            UserID = userId;
        }

        #region Methods
        #endregion
    }
}