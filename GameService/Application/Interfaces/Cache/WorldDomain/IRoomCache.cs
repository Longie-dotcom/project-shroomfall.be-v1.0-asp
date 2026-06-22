using Domain.Definition.WorldDomain;

namespace Application.Interfaces.Cache.WorldDomain
{
    public interface IRoomCache
    {
        void Load(
            IEnumerable<RoomDefinition> data);
        IReadOnlyCollection<RoomDefinition> GetAll();
        RoomDefinition? Get(
            string id);
        Cell? GetTopCell(
            string roomId,
            int worldX,
            int worldY);
    }
}
