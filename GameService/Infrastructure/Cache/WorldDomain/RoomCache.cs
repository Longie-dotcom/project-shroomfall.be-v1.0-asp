using Application.Interfaces.Cache.WorldDomain;
using Contract;
using Domain.Definition.WorldDomain;
using Domain.Shared;

namespace Infrastructure.Cache.WorldDomain
{
    public class RoomCache : IRoomCache
    {
        #region Attributes
        private Dictionary<string, RoomDefinition> map = new();
        private readonly Dictionary<
            string,
            Dictionary<
                int,
                Dictionary<
                    (int cx, int cy),
                    Cell[,]
                >
            >
        > cellIndex = new();

        private readonly Dictionary<string, List<int>> sortedLayers = new();
        #endregion

        #region Properties
        #endregion

        public RoomCache() { }

        #region Methods
        public void Load(
            IEnumerable<RoomDefinition> data)
        {
            map = data.ToDictionary(r => r.ID);

            cellIndex.Clear();

            foreach (var room in data)
            {
                IndexRoom(room);
            }
        }

        public IReadOnlyCollection<RoomDefinition> GetAll()
        {
            return map.Values.ToList();
        }

        public RoomDefinition? Get(
            string id)
        {
            return map.TryGetValue(id, out var room)
                ? room
                : null;
        }

        public Cell? GetTopCell(
            string roomId,
            int worldX,
            int worldY)
        {
            if (!cellIndex.TryGetValue(roomId, out var layers))
                return null;

            if (!sortedLayers.TryGetValue(roomId, out var sorted))
                return null;

            var (cx, cy, x, y) = ChunkMath.ToChunk(worldX, worldY, Constraint.CHUNK_SIZE);

            foreach (var z in sorted)
            {
                if (!layers.TryGetValue(z, out var chunks))
                    continue;

                if (!chunks.TryGetValue((cx, cy), out var grid))
                    continue;

                var cell = grid[x, y];

                if (cell != null)
                    return cell;
            }

            return null;
        }

        private void IndexRoom(
            RoomDefinition room)
        {
            var layerMap = new Dictionary<int, Dictionary<(int cx, int cy), Cell[,]>>();

            foreach (var layerGroup in room.Cells.GroupBy(c => c.Z))
            {
                int z = layerGroup.Key;

                var chunks = new Dictionary<(int cx, int cy), Cell[,]>();

                foreach (var chunkGroup in layerGroup.GroupBy(c =>
                {
                    var (cx, cy) = ChunkMath.ToChunkOnly(c.X, c.Y, Constraint.CHUNK_SIZE);

                    return (cx, cy);
                }))
                {
                    var (cx, cy) = chunkGroup.Key;

                    var grid = new Cell[Constraint.CHUNK_SIZE, Constraint.CHUNK_SIZE];

                    foreach (var cell in chunkGroup)
                    {
                        int lx = cell.X - cx * Constraint.CHUNK_SIZE;
                        int ly = cell.Y - cy * Constraint.CHUNK_SIZE;

                        grid[lx, ly] = cell;
                    }

                    chunks[(cx, cy)] = grid;
                }

                layerMap[z] = chunks;
            }

            cellIndex[room.ID] = layerMap;

            // IMPORTANT: sort layers once
            sortedLayers[room.ID] = layerMap.Keys
                .OrderByDescending(z => z)
                .ToList();
        }
        #endregion
    }
}