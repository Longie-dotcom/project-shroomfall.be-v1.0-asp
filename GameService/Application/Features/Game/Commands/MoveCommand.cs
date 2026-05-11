using Application.DTO.Game;

namespace Application.Features.Game.Commands
{
    public class MoveCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public MoveDTO DTO { get; }
        #endregion

        public MoveCommand(
            string userId,
            MoveDTO dto)
        {
            UserID = userId;
            DTO = dto;
        }

        #region Methods
        #endregion
    }
}