using Application.Features.Abstraction;
using Contract.DTO.Game;

namespace Application.Features.Game.Commands
{
    public class UseItemCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public UseItemDTO DTO { get; }
        #endregion

        public UseItemCommand(
            string userId, 
            UseItemDTO dto)
        {
            UserID = userId;
            DTO = dto;
        }

        #region Methods
        #endregion
    }
}