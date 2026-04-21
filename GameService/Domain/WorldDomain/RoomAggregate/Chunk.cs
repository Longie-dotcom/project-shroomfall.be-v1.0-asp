namespace Domain.WorldDomain.RoomAggregate
{
    public class Chunk
    {
        #region Attributes
        #endregion

        #region Properties
        public int X { get; private set; }
        public int Y { get; private set; }
        public Cell[][] Cells { get; private set; }
        #endregion

        public Chunk(
            int x,
            int y,
            Cell[][] cells)
        {
            X = x;
            Y = y;
            Cells = cells;
        }

        #region Methods
        #endregion
    }
}
