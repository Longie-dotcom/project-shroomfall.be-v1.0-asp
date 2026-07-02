using Contract.DTO.Domain.Definition;

namespace Application.Features.Design.Commands
{
    public class UpsertRoomDefinitionCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public RoomDefinitionDTO Room { get; }
        public List<CellDefinitionDTO> Cells { get; }
        public List<EntitySpawnRuleDefinitionDTO> EntitySpawnRules { get; }
        #endregion

        public UpsertRoomDefinitionCommand(
            RoomDefinitionDTO room,
            List<CellDefinitionDTO> cells,
            List<EntitySpawnRuleDefinitionDTO> entitySpawnRules)
        {
            Room = room;
            Cells = cells;
            EntitySpawnRules = entitySpawnRules;
        }

        #region Methods
        #endregion
    }
}