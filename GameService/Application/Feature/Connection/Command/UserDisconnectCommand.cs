namespace Application.Feature.Connection.Command
{
    public class UserDisconnectCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public string ConnectionID { get; }
        #endregion

        public UserDisconnectCommand(
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