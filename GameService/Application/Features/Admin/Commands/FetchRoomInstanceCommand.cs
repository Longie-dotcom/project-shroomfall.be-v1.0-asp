namespace Application.Features.Admin.Commands
{
    public class FetchRoomInstanceCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; set; }
        #endregion

        public FetchRoomInstanceCommand(
            string userId)
        {
            UserID = userId;
        }

        #region Methods
        #endregion
    }
}