using Contract.DTO.Feature.Design.Command;

namespace Application.Features.Design.Commands
{
    public class UpsertItemDefinitionCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public UpsertItemDefinitionDTO DTO { get; }
        #endregion

        public UpsertItemDefinitionCommand(
            string userId,
            UpsertItemDefinitionDTO dto)
        {
            UserID = userId;
            DTO = dto;
        }

        #region Methods
        #endregion
    }
}