namespace Application.Features.Connection.Handlers
{
    public class ChangeRoomCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public string NewRoomSpatailID { get; }
        #endregion

        public ChangeRoomCommand(
            string userId, string 
            newRoomSpatailId)
        {
            UserID = userId;
            NewRoomSpatailID = newRoomSpatailId;
        }

        #region Methods
        #endregion
    }
}