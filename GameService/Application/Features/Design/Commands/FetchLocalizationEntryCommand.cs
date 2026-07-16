using Contract.DTO.Feature.Design.Command;

namespace Application.Features.Design.Commands
{
    public class FetchLocalizationEntryCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public LocalizationEntryQueryDTO Queries { get; }
        #endregion

        public FetchLocalizationEntryCommand(
            string userId,
            LocalizationEntryQueryDTO queries)
        {
            Queries = queries;
            UserID = userId;
        }

        #region Methods
        #endregion
    }
}