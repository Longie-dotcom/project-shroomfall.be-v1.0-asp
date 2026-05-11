using Application.DTO.Connection;

namespace Application.Features.Connection.Commands
{
    public class CreateSessionCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public CreateSessionDTO DTO { get; } 
        #endregion

        public CreateSessionCommand(
            string userId,
            CreateSessionDTO dto)
        {
            UserID = userId;
            DTO = dto;
        }

        #region Methods
        #endregion
    }
}