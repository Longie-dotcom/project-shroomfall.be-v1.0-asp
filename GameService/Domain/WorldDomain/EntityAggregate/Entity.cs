using Domain.Common;
using Domain.WorldDomain.Enum;

namespace Domain.WorldDomain.EntityAggregate
{
    public class Entity
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; }
        public EntityType Type { get; private set; }
        public string RoomID { get; private set; }
        public string LayerID { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 Direction { get; private set; }
        #endregion

        public Entity(
            string id,
            EntityType type,
            string roomId,
            string layerId,
            Vector2 position,
            Vector2 direction)
        {
            ID = id;
            Type = type;
            RoomID = roomId;
            LayerID = layerId;
            Position = position;
            Direction = direction;
        }

        #region Methods
        #endregion
    }
}
