using Domain.DomainException;
using Domain.Shared;

namespace Domain.Definition.WorldDomain
{
    public class SpawnArea
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; }
        public int MinX { get; private set; }
        public int MinY { get; private set; }
        public int MaxX { get; private set; }
        public int MaxY { get; private set; }
        public int MinCount { get; private set; }
        public int MaxCount { get; private set; }
        public float Weight { get; private set; }
        public string EntitySpawnRuleID { get; private set; }

        public EntitySpawnRule EntitySpawnRule { get; private set; }
        #endregion
        
        protected SpawnArea() 
        {
        
        }

        public SpawnArea(
            string id,
            int minX,
            int minY,
            int maxX,
            int maxY,
            int minCount,
            int maxCount,
            float weight,
            string entitySpawnRuleId)
        {
            if (string.IsNullOrEmpty(id))
                throw new BadRequest(ResponseCode.SpawnArea_InvalidId);

            if (maxX < minX || maxY < minY)
                throw new BadRequest(ResponseCode.SpawnArea_InvalidBounds);

            if (minCount < 0)
                throw new BadRequest(ResponseCode.SpawnArea_InvalidMinCount);

            if (maxCount < minCount)
                throw new BadRequest(ResponseCode.SpawnArea_InvalidMaxCount);

            if (weight < 0)
                throw new BadRequest(ResponseCode.SpawnArea_InvalidWeight);

            if (string.IsNullOrEmpty(entitySpawnRuleId))
                throw new BadRequest(ResponseCode.SpawnArea_InvalidEntitySpawnRuleId);

            ID = id;
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
            MinCount = minCount;
            MaxCount = maxCount;
            Weight = weight;
            EntitySpawnRuleID = entitySpawnRuleId;
        }

        #region Methods
        #endregion
    }
}