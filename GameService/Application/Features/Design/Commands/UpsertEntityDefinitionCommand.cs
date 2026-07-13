using Contract.DTO.Definition.EntityDomain.Component;

namespace Application.Features.Design.Commands
{
    public class UpsertEntityDefinitionCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public EntityDefinitionDTO DTO { get; }
        #endregion

        public UpsertEntityDefinitionCommand(
            string userId,
            EntityDefinitionDTO dto)
        {
            UserID = userId;
            DTO = dto;
        }

        #region Methods
        #endregion
    }
}