using Contract.DTO.Design;

namespace Application.Features.Design.Commands
{
    public class FetchEntityDefinitionCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public FetchAllEntitiesQueriesDTO Queries { get; }
        #endregion

        public FetchEntityDefinitionCommand(
            string userId,
            FetchAllEntitiesQueriesDTO queries)
        {
            Queries = queries;
            UserID = userId;
        }

        #region Methods
        #endregion
    }
}