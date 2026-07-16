using Contract.DTO.Feature.Design.Command;

namespace Application.Features.Design.Commands
{
    public class FetchRoomDefinitionCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public RoomDefinitionQueryDTO Queries { get; }
        #endregion

        public FetchRoomDefinitionCommand(
            string userId,
            RoomDefinitionQueryDTO queries)
        {
            UserID = userId;
            Queries = queries;
        }

        #region Methods
        #endregion
    }
}