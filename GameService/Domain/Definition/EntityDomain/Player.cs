using Domain.Definition.EntityDomain.Component;
using Domain.Definition.EntityDomain.Enum;
using Domain.Definition.LocalizationDomain;

namespace Domain.Definition.EntityDomain
{
    public class Player : Creature
    {
        #region Attributes
        #endregion

        #region Properties
        public PlayerAppearance PlayerAppearance { get; private set; }
        #endregion

        protected Player()
        {

        }

        public Player(
            string id,
            EntityType type,
            LocalizedText localizedText,
            PlayerAppearance playerAppearance,
            Collision collision,
            string characteristicId,
            string inventoryId,
            int level) : base(
                id,
                type,
                localizedText,
                playerAppearance,
                collision,
                characteristicId,
                inventoryId,
                level)
        {
            PlayerAppearance = playerAppearance;
        }

        #region Methods
        #endregion
    }
}
