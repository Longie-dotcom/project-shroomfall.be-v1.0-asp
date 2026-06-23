using Contract.DTO.Design;

namespace Application.Features.Design.Commands
{
    public class UpsertEntityDefinitionCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public UpsertEntityDefinitionDTO DTO { get; }
        #endregion

        public UpsertEntityDefinitionCommand(
            string userId,
            UpsertEntityDefinitionDTO dto)
        {
            UserID = userId;
            DTO = dto;
        }

        #region Methods
        #endregion
    }
}