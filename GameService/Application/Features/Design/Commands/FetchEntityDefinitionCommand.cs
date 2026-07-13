using Contract.DTO.Feature.Design.Command;

namespace Application.Features.Design.Commands
{
    public class FetchEntityDefinitionCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public EntityDefinitionQueryDTO Queries { get; }
        #endregion

        public FetchEntityDefinitionCommand(
            string userId,
            EntityDefinitionQueryDTO queries)
        {
            Queries = queries;
            UserID = userId;
        }

        #region Methods
        #endregion
    }
}