using Application.Interface.Cache.WorldDomain;
using Contract;
using Contract.DTO.Definition.WorldDomain;
using Domain.Shared;

namespace Infrastructure.Cache.WorldDomain
{
    public class RoomCache : IRoomCache
    {
        #region Attributes
        private Dictionary<string, RoomDefinitionDTO> map = new();
        private readonly Dictionary<string, Dictionary<int, Dictionary<(int cx, int cy), CellDTO[,]>>> cellIndex = new();
        private readonly Dictionary<string, List<EntitySpawnRuleDTO>> spawnRuleIndex = new();
        private readonly Dictionary<string, List<int>> sortedLayers = new();
        #endregion

        #region Properties
        #endregion

        public RoomCache() { }

        #region Methods
        public void Load(
            IEnumerable<RoomDefinitionDTO> roomData,
            IEnumerable<CellDTO> cellData,
            IEnumerable<EntitySpawnRuleDTO> entitySpawnRuleData)
        {
            // ROOM DEFINITIONS
            map = roomData.ToDictionary(
                room => room.Id);

            // CLEAR OLD INDEXES
            cellIndex.Clear();
            spawnRuleIndex.Clear();
            sortedLayers.Clear();

            // CELL INDEX
            foreach (var roomGroup in cellData.GroupBy(
                cell => cell.RoomDefinitionID))
            {
                IndexCells(
                    roomGroup.Key,
                    roomGroup);
            }

            // SPAWN RULE INDEX
            foreach (var roomGroup in entitySpawnRuleData.GroupBy(
                rule => rule.RoomDefinitionID))
            {
                spawnRuleIndex[roomGroup.Key] =
                    roomGroup.ToList();
            }
        }

        public IReadOnlyCollection<RoomDefinitionDTO> GetAll()
        {
            return map.Values.ToList();
        }

        public RoomDefinitionDTO? Get(
            string id)
        {
            return map.TryGetValue(id, out var room)
                ? room
                : null;
        }

        public CellDTO? GetTopCell(
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

        public IReadOnlyList<EntitySpawnRuleDTO> GetSpawnRules(
            string roomId)
        {
            return spawnRuleIndex.TryGetValue(
                roomId,
                out var rules)
                ? rules
                : Array.Empty<EntitySpawnRuleDTO>();
        }

        private void IndexCells(
            string roomId,
            IEnumerable<CellDTO> cells)
        {
            var layerMap = new Dictionary<int, Dictionary<(int cx, int cy), CellDTO[,]>>();

            foreach (var layerGroup in cells.GroupBy(c => c.Z))
            {
                int z = layerGroup.Key;

                var chunks = new Dictionary<(int cx, int cy), CellDTO[,]>();

                foreach (var chunkGroup in layerGroup.GroupBy(c =>
                {
                    var (cx, cy) = ChunkMath.ToChunkOnly(c.X, c.Y, Constraint.CHUNK_SIZE);

                    return (cx, cy);
                }))
                {
                    var (cx, cy) = chunkGroup.Key;

                    var grid = new CellDTO[Constraint.CHUNK_SIZE, Constraint.CHUNK_SIZE];

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

            cellIndex[roomId] = layerMap;

            // IMPORTANT: sort layers once
            sortedLayers[roomId] = layerMap.Keys
                .OrderByDescending(z => z)
                .ToList();
        }
        #endregion
    }
}