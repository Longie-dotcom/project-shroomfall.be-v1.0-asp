using Domain.Definition.WorldDomain;

namespace Application.Interfaces.Cache
{
    public interface IRoomCache
    {
        void Load(
            IEnumerable<Room> data);
        IReadOnlyCollection<Room> GetAll();
        Room? Get(
            string id);
        Cell? GetTopCell(
            string roomId,
            int worldX,
            int worldY);
    }
}
