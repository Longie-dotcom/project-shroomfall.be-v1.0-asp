using Contract.DTO.Identity;

namespace Application.Features.Identity.Commands
{
    public class LoginCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public LoginDTO DTO { get; }
        #endregion

        public LoginCommand(
            LoginDTO dto)
        {
            DTO = dto;
        }

        #region Methods
        #endregion
    }
}