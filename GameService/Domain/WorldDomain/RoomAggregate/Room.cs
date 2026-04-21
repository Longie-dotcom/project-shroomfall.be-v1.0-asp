using Domain.WorldDomain.EntityAggregate;
using Domain.WorldDomain.Enum;

namespace Domain.WorldDomain.RoomAggregate
{
    public class Room
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; }
        public RoomType Type { get; private set; }
        public Dictionary<string, Entity> Entities { get; private set; }
        public Dictionary<string, Layer> Layers { get; private set; }
        #endregion

        public Room(
            string id,
            RoomType type,
            Dictionary<string, Entity> entities,
            Dictionary<string, Layer> layers)
        {
            ID = id;
            Type = type;
            Entities = entities;
            Layers = layers;
        }

        #region Methods
        #endregion
    }
}
