namespace Application.Feature.Connection.Command
{
    public class FetchSessionCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        #endregion

        public FetchSessionCommand(
            string userId)
        {
            UserID = userId;
        }

        #region Methods
        #endregion
    }
}