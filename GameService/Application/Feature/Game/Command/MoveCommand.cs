using Contract.DTO.Feature.Game.Command;

namespace Application.Feature.Game.Command
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