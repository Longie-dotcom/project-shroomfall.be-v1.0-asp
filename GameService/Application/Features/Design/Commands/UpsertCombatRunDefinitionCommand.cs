using Contract.DTO.Definition.WorldDomain;

namespace Application.Features.Design.Commands
{
    public class UpsertCombatRunDefinitionCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public CombatRunDefinitionDTO DTO { get; }
        #endregion

        public UpsertCombatRunDefinitionCommand(
            string userId,
            CombatRunDefinitionDTO dto)
        {
            UserID = userId;
            DTO = dto;
        }

        #region Methods
        #endregion
    }
}
