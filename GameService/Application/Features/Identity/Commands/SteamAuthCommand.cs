using Application.Features.Abstraction;
using Contract.DTO.Identity;

namespace Application.Features.Identity.Commands
{
    public class SteamAuthCommand : ICommand<SteamAuthDTO>
    {
        #region Attributes
        #endregion

        #region Properties
        public SteamAuthDTO DTO { get; }
        #endregion

        public SteamAuthCommand(
            SteamAuthDTO dto)
        {
            DTO = dto;
        }

        #region Methods
        #endregion
    }
}