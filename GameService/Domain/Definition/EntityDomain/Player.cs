using Contract.Enum.EntityDomain;
using Domain.Definition.EntityDomain.Component;
using Domain.Definition.LocalizationDomain;

namespace Domain.Definition.EntityDomain
{
    public class Player : Creature
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        protected Player()
        {

        }

        public Player(
            string id,
            EntityType type,
            LocalizedText localizedText,
            Appearance appearance,
            Collision collision,
            string characteristicId,
            string inventoryId,
            int level) : base(
                id,
                type,
                localizedText,
                appearance,
                collision,
                characteristicId,
                inventoryId,
                level)
        {

        }

        #region Methods
        #endregion
    }
}
