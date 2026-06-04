using Contract.Enum.WorldDomain;
using Domain.DomainException;
using Domain.Shared;

namespace Domain.Definition.WorldDomain
{
    public class Cell
    {
        #region Attributes
        #endregion

        #region Properties
        public string RoomID { get; private set; }
        public string TileID { get; private set; }
        public CellType Type { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }

        public Room Room { get; private set; }
        #endregion

        protected Cell() 
        {
        
        }

        public Cell(
            string roomId, 
            string tileId, 
            CellType type,
            int x, 
            int y, 
            int z)
        {
            if (string.IsNullOrWhiteSpace(roomId))
                throw new BadRequest(ResponseCode.Cell_InvalidRoomId);

            if (string.IsNullOrWhiteSpace(tileId))
                throw new BadRequest(ResponseCode.Cell_InvalidTileId);

            RoomID = roomId;
            TileID = tileId;
            Type = type;
            X = x;
            Y = y;
            Z = z;
        }

        #region Methods
        #endregion
    }
}