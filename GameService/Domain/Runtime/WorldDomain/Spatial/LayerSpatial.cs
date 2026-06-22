using Contract;
using Domain.Runtime.EntityDomain;
using Domain.Shared;

namespace Domain.Runtime.WorldDomain.Spatial
{
    public class LayerSpatial
    {
        #region Attributes
        private readonly Dictionary<(int chunkX, int chunkY), ChunkSpatial> chunks = new();
        #endregion

        #region Properties
        #endregion

        public LayerSpatial() { }

        #region Methods
        public void AddEntity(
            EntityInstance entity,
            int chunkX, int chunkY,
            int x, int y)
        {
            var chunk = GetOrCreateChunk(chunkX, chunkY);

            chunk.AddEntity(entity, x, y);
        }

        public void RemoveEntity(
            EntityInstance entity, 
            int chunkX, int chunkY,
            int x, int y)
        {
            if (!chunks.TryGetValue((chunkX, chunkY), out var chunk))
                return;

            chunk.RemoveEntity(entity, x, y);
        }

        public IEnumerable<EntityInstance> Query(
            int x, int y)
        {
            var (chunkX, chunkY, localX, localY) = ChunkMath.ToChunk(x, y, Constraint.CHUNK_SIZE);

            if (!chunks.TryGetValue((chunkX, chunkY), out var chunk))
                return Enumerable.Empty<EntityInstance>();

            return chunk.Query(localX, localY);
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