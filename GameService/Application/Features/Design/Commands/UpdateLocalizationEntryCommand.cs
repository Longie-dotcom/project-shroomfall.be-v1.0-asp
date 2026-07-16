using Contract.DTO.Definition.LocalizationDomain;

namespace Application.Features.Design.Commands
{
    public class UpdateLocalizationEntryCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public LocalizationEntryDTO DTO { get; }
        #endregion

        public UpdateLocalizationEntryCommand(
            string userId,
            LocalizationEntryDTO dto)
        {
            UserID = userId;
            DTO = dto;
        }

        #region Methods
        #endregion
    }
}