namespace Domain.Shared
{
    public static class ChunkMath
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static (int cx, int cy, int x, int y) ToChunk(int worldX, int worldY, int size)
        {
            int cx = Math.DivRem(worldX, size, out int x);
            int cy = Math.DivRem(worldY, size, out int y);

            if (x < 0) { cx--; x += size; }
            if (y < 0) { cy--; y += size; }

            return (cx, cy, x, y);
        }

        public static (int cx, int cy) ToChunkOnly(int worldX, int worldY, int size)
        {
            return (
                (int)Math.Floor((float)worldX / size),
                (int)Math.Floor((float)worldY / size)
            );
        }
        #endregion
    }
}