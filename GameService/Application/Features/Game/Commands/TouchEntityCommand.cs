namespace Application.Features.Game.Commands
{
    public class TouchEntityCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public string TouchedEntityInstanceID { get; }
        #endregion

        public TouchEntityCommand(
            string userId, 
            string touchedEntityInstanceId)
        {
            UserID = userId;
            TouchedEntityInstanceID = touchedEntityInstanceId;
        }

        #region Methods
        #endregion
    }
}