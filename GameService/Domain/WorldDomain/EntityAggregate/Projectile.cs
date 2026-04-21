using Domain.Common;
using Domain.WorldDomain.Enum;

namespace Domain.WorldDomain.EntityAggregate
{
    public class Projectile : Entity
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public Projectile(
            string id, 
            EntityType type, 
            string roomId, 
            string layerId, 
            Vector2 position,
            Vector2 direction) : base(
                id, 
                type, 
                roomId, 
                layerId, 
                position,
                direction)
        {

        }

        #region Methods
        #endregion
    }
}
