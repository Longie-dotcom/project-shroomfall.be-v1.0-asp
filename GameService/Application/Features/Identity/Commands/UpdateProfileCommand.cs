using Contract.DTO.Feature.Identity.Command;

namespace Application.Features.Identity.Commands
{
    public class UpdateProfileCommand
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