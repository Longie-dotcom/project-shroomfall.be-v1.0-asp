using Application.Interfaces.Cache;
using Domain.Definition.WorldDomain;
using Domain.Definition.WorldDomain.Enum;

namespace Infrastructure.Cache
{
    public class TileCache : ITileCache
    {
        #region Attributes
        private Dictionary<string, Tile> map = new();
        private Dictionary<TileType, List<Tile>> tilesByType = new();
        #endregion

        #region Properties
        #endregion

        public TileCache()
        {

        }

        #region Methods
        public void Load(
            IEnumerable<Tile> data)
        {
            map = data.ToDictionary(t => t.ID);
            tilesByType.Clear();

            foreach (var tile in data)
            {
                if (!tilesByType.TryGetValue(tile.Type, out var list))
                {
                    list = new List<Tile>();
                    tilesByType[tile.Type] = list;
                }

                list.Add(tile);
            }
        }

        public IReadOnlyCollection<Tile> GetAll()
        {
            return map.Values.ToList();
        }

        public Tile? Get(
            string id)
        {
            return map.TryGetValue(id, out var tile) 
                ? tile 
                : null;
        }

        public IReadOnlyCollection<Tile> GetByType(
            TileType type)
        {
            if (tilesByType.TryGetValue(type, out var list))
                return list;

            return new List<Tile>();
        }
        #endregion
    }
}