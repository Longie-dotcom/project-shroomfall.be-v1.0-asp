using Domain.Shared;

namespace Domain.Runtime.WorldDomain
{
    public class LayerSpatial
    {
        #region Attributes
        private readonly Dictionary<(int chunkX, int chunkY), ChunkSpatial> chunks = new();
        #endregion

        #region Properties
        #endregion

        public LayerSpatial()
        {

        }

        #region Methods
        public void AddEntity(string entityId, int chunkX, int chunkY, int x, int y)
        {
            var chunk = GetOrCreateChunk(chunkX, chunkY);

            chunk.AddEntity(entityId, x, y);
        }

        public void RemoveEntity(string entityId, int chunkX, int chunkY, int x, int y)
        {
            if (!chunks.TryGetValue((chunkX, chunkY), out var chunk))
                return;

            chunk.RemoveEntity(entityId, x, y);
        }

        public IEnumerable<string> Query(int x, int y)
        {
            var (chunkX, chunkY) = ChunkMath.ToChunkOnly(x, y, Constraint.CHUNK_SIZE);

            if (!chunks.TryGetValue((chunkX, chunkY), out var chunk))
                return Enumerable.Empty<string>();

            return chunk.Query(
                x % Constraint.CHUNK_SIZE,
                y % Constraint.CHUNK_SIZE
            );
        }

        private ChunkSpatial GetOrCreateChunk(int x, int y)
        {
            if (!chunks.TryGetValue((x, y), out var chunk))
            {
                chunk = new ChunkSpatial();
                chunks[(x, y)] = chunk;
            }
            return chunk;
        }
        #endregion
    }
}