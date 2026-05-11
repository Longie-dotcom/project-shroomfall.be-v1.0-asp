using Application.DTO.Design;

namespace Application.Features.Design.Commands
{
    public class UpdateDefinitionCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public UpdateDefinitionDTO DTO { get; }
        #endregion

        public UpdateDefinitionCommand(
            string userId,
            UpdateDefinitionDTO dto)
        {
            UserID = userId;
            DTO = dto;
        }

        #region Methods
        #endregion
    }
}