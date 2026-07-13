using Contract.DTO.Definition.MetaDomain;

namespace Application.Features.Design.Commands
{
    public class UpsertEffectDefinitionCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public EffectDefinitionDTO DTO { get; }
        #endregion

        public UpsertEffectDefinitionCommand(
            string userId,
            EffectDefinitionDTO dto)
        {
            UserID = userId;
            DTO = dto;
        }

        #region Methods
        #endregion
    }
}