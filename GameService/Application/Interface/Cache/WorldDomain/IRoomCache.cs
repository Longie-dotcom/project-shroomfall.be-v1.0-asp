using Contract.DTO.Definition.WorldDomain;

namespace Application.Interface.Cache.WorldDomain
{
    public interface IRoomCache
    {
        void Load(
            IEnumerable<RoomDefinitionDTO> roomData,
            IEnumerable<CellDTO> cellData,
            IEnumerable<EntitySpawnRuleDTO> entitySpawnRuleData);
        IReadOnlyCollection<RoomDefinitionDTO> GetAll();
        RoomDefinitionDTO? Get(
            string id);
        CellDTO? GetTopCell(
            string roomId,
            int worldX,
            int worldY);
        IReadOnlyList<EntitySpawnRuleDTO> GetSpawnRules(
            string roomId);
    }
}
