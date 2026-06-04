using Application.Features.Abstraction;
using Contract.DTO.Identity;

namespace Application.Features.Identity.Commands
{
    public class RefreshTokenCommand : ICommand<RefreshTokenDTO>
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public RefreshTokenDTO DTO { get; }
        #endregion

        public RefreshTokenCommand(
            string userId,
            RefreshTokenDTO dto)
        {
            UserID = userId;
            DTO = dto;
        }

        #region Methods
        #endregion
    }
}