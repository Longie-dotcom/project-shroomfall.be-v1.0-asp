namespace Application.Feature.Game.Command
{
    public class BackHomeCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        #endregion

        public BackHomeCommand(
            string userId)
        {
            UserID = userId;
        }

        #region Methods
        #endregion
    }
}