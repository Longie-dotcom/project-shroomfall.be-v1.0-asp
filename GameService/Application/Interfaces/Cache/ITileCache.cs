using Domain.Definition.WorldDomain;
using Domain.Definition.WorldDomain.Enum;

namespace Application.Interfaces.Cache
{
    public interface ITileCache
    {
        void Load(
            IEnumerable<Tile> data);
        IReadOnlyCollection<Tile> GetAll();
        Tile? Get(
            string id);
        IReadOnlyCollection<Tile> GetByType(
            TileType type);
    }
}
