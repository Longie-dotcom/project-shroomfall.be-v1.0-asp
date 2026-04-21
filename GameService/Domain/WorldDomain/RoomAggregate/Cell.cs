using Domain.WorldDomain.Enum;

namespace Domain.WorldDomain.RoomAggregate
{
    public class Cell
    {
        #region Attributes
        #endregion

        #region Properties
        public CellType Type { get; private set; }
        public bool IsWalkable { get; private set; }
        #endregion

        public Cell(
            CellType type,
            bool isWalkable)
        {
            Type = type;
            IsWalkable = isWalkable;
        }

        #region Methods
        #endregion
    }
}
