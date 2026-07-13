using Contract.DTO.Feature.Identity.Command;

namespace Application.Features.Identity.Commands
{
    public class SteamAuthCommand
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