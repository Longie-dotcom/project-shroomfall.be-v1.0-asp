using Domain.WorldDomain.Enum;

namespace Domain.WorldDomain.RoomAggregate
{
    public class Layer
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; }
        public LayerType Type { get; private set; }
        public Dictionary<(int, int), Chunk> Chunks { get; private set; }
        #endregion

        public Layer(
            string id,
            LayerType type,
            Dictionary<(int, int), Chunk> chunks)
        {
            ID = id;
            Type = type;
            Chunks = chunks;
        }

        #region Methods
        public Chunk? TryGetChunk(int chunkX, int chunkY)
        {
            Chunks.TryGetValue((chunkX, chunkY), out var chunk);
            return chunk;
        }
        #endregion
    }
}
