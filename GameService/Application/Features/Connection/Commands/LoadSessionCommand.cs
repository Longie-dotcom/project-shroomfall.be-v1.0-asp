using Application.DTO.Connection;

namespace Application.Features.Connection.Commands
{
    public class LoadSessionCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public LoadSessionDTO DTO { get; }
        #endregion

        public LoadSessionCommand(
            string userId,
            LoadSessionDTO dto)
        {
            UserID = userId;
            DTO = dto;
        }

        #region Methods
        #endregion
    }
}