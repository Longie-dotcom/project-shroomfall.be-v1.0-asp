using Contract.DTO.Feature.Game.Command;

namespace Application.Features.Game.Commands
{
    public class UpdateAppearanceCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public UpdatePlayerAppearanceDTO DTO { get; }
        #endregion

        public UpdateAppearanceCommand(
            string userId,
            UpdatePlayerAppearanceDTO dto)
        {
            UserID = userId;
            DTO = dto;
        }

        #region Methods
        #endregion
    }
}