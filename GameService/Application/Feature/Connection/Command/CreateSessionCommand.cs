using Contract.DTO.Feature.Connection.Command;

namespace Application.Feature.Connection.Command
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