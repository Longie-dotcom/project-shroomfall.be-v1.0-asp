using Contract.DTO.Design;

namespace Application.Features.Design.Commands
{
    public class FetchItemDefinitionCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public ItemDefinitionQueryDTO Queries { get; }
        #endregion

        public FetchItemDefinitionCommand(
            string userId,
            ItemDefinitionQueryDTO queries)
        {
            Queries = queries;
            UserID = userId;
        }

        #region Methods
        #endregion
    }
}