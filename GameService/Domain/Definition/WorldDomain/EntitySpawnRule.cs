using Contract.Enum.WorldDomain;
using Domain.Definition.EntityDomain;
using Domain.DomainException;
using Domain.Shared;

namespace Domain.Definition.WorldDomain
{
    public class EntitySpawnRule
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; }
        public SpawnRuleType Type { get; private set; }
        public string RoomID { get; private set; }
        public string EntityID { get; private set; }

        public Room Room { get; private set; }
        public Entity Entity { get; private set; }
        public ICollection<SpawnArea> SpawnAreas { get; private set; } = new List<SpawnArea>();
        #endregion

        protected EntitySpawnRule() 
        { 
        
        }

        public EntitySpawnRule(
            string id,
            SpawnRuleType type,
            string roomId,
            string entityId)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadRequest(ResponseCode.EntitySpawnRule_InvalidId);

            if (string.IsNullOrWhiteSpace(roomId))
                throw new BadRequest(ResponseCode.EntitySpawnRule_InvalidRoomId);

            if (string.IsNullOrWhiteSpace(entityId))
                throw new BadRequest(ResponseCode.EntitySpawnRule_InvalidEntityId);

            ID = id;
            Type = type;
            RoomID = roomId;
            EntityID = entityId;
        }

        #region Methods
        #endregion
    }
}