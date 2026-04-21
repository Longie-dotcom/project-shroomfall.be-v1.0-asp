using Domain.AttributeDomain.Attribute;
using Domain.Common;
using Domain.WorldDomain.Enum;

namespace Domain.WorldDomain.EntityAggregate
{
    public class Creature : Entity
    {
        #region Attributes
        #endregion

        #region Properties
        public Characteristic Characteristic { get; private set; }
        public Stats Stats { get; private set; }
        public List<Effect> ActiveEffects { get; private set; }
        #endregion

        public Creature(
            string id,
            EntityType type,
            string roomId,
            string layerId,
            Vector2 position,
            Vector2 direction,
            Characteristic characteristic,
            Stats stats,
            List<Effect> activeEffects) : base(
                id,
                type,
                roomId,
                layerId,
                position,
                direction)
        {
            Characteristic = characteristic;
            Stats = stats;
            ActiveEffects = activeEffects;
        }

        #region Methods
        #endregion
    }
}
