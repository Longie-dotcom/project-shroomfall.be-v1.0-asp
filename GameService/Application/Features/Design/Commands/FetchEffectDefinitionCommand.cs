using Contract.DTO.Feature.Design.Command;

namespace Application.Features.Design.Commands
{
    public class FetchEffectDefinitionCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public EffectDefinitionQueryDTO Queries { get; }
        #endregion

        public FetchEffectDefinitionCommand(
            string userId,
            EffectDefinitionQueryDTO queries)
        {
            Queries = queries;
            UserID = userId;
        }

        #region Methods
        #endregion
    }
}