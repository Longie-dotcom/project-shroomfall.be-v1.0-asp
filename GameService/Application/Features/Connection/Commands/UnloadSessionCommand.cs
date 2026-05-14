namespace Application.Features.Connection.Commands
{
    public class UnloadSessionCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public string ConnectionID { get; }
        #endregion

        public UnloadSessionCommand(
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