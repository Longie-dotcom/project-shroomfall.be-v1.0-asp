namespace Application.Feature.Connection.Command
{
    public class UserConnectCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public string ConnectionID { get; }
        #endregion

        public UserConnectCommand(
            string userId, 
            string connectionId)
        {
            UserID = userId;
            ConnectionID = connectionId;
        }

        #region Methods
        #endregion
    }
}