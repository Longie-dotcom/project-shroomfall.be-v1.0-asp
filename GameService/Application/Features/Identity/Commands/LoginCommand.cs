using Application.Features.Abstraction;
using Contract.DTO.Identity;

namespace Application.Features.Identity.Commands
{
    public class LoginCommand : ICommand<LoginDTO>
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