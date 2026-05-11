using Application.DTO.Identity;
using Application.Features.Abstraction;

namespace Application.Features.Identity.Commands
{
    public class RegisterCommand : ICommand<RegisterDTO>
    {
        #region Attributes
        #endregion

        #region Properties
        public RegisterDTO DTO { get; }
        #endregion

        public RegisterCommand(
            RegisterDTO dTO)
        {
            DTO = dTO;
        }

        #region Methods
        #endregion
    }
}