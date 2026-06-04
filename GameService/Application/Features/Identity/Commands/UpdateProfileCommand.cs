using Application.Features.Abstraction;
using Contract.DTO.Identity;

namespace Application.Features.Identity.Commands
{
    public class UpdateProfileCommand : ICommand<UpdateProfileDTO>
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public UpdateProfileDTO DTO { get; }
        #endregion

        public UpdateProfileCommand(
            string userId,
            UpdateProfileDTO dto)
        {
            UserID = userId;
            DTO = dto;
        }

        #region Methods
        #endregion
    }
}