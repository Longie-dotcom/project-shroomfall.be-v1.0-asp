using Contract.DTO.Feature.Design.Command;

namespace Application.Feature.Game.Command
{
    public class UserRefreshCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public UserRefreshDTO DTO { get; }
        #endregion

        public UserRefreshCommand(
            string userId,
            UserRefreshDTO dto)
        {
            UserID = userId;
            DTO = dto;  
        }

        #region Methods
        #endregion
    }
}