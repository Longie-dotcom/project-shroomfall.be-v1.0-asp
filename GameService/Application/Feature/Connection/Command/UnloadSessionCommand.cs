namespace Application.Feature.Connection.Command
{
    public class UnloadSessionCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        #endregion

        public UnloadSessionCommand(
            string userId)
        {
            UserID = userId;
        }

        #region Methods
        #endregion
    }
}