namespace Application.Features.Design.Commands
{
    public class FetchLocaleCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        #endregion

        public FetchLocaleCommand(
            string userId)
        {
            UserID = userId;
        }

        #region Methods
        #endregion
    }
}