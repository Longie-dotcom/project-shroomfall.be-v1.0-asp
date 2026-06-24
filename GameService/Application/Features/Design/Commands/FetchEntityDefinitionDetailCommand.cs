namespace Application.Features.Design.Commands
{
    public class FetchEntityDefinitionDetailCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public string ID { get; }
        #endregion

        public FetchEntityDefinitionDetailCommand(
            string userId,
            string id)
        {
            UserID = userId;
            ID = id;
        }

        #region Methods
        #endregion
    }
}