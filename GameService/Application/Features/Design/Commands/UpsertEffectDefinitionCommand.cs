using Contract.DTO.Design;

namespace Application.Features.Design.Commands
{
    public class UpsertEffectDefinitionCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public UpsertEffectDefinitionDTO DTO { get; }
        #endregion

        public UpsertEffectDefinitionCommand(
            string userId,
            UpsertEffectDefinitionDTO dto)
        {
            UserID = userId;
            DTO = dto;
        }

        #region Methods
        #endregion
    }
}