using Application.Features.Abstraction;
using Contract.DTO.Game;
using Contract.DTO.Runtime;

namespace Application.Features.Game.Commands
{
    public class UpdateAppearanceCommand : ICommand<PlayerAppearanceRuntimeDTO>
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public UpdatePlayerAppearanceDTO DTO { get; }
        #endregion

        public UpdateAppearanceCommand(
            string userId,
            UpdatePlayerAppearanceDTO dto)
        {
            UserID = userId;
            DTO = dto;
        }

        #region Methods
        #endregion
    }
}