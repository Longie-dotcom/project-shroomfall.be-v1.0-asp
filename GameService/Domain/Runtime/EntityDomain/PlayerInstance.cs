using Domain.Abstraction;
using Domain.Common;
using Domain.Runtime.AttributeDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.ItemDomain;

namespace Domain.Runtime.EntityDomain
{
    public class PlayerInstance : CreatureInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public PlayerAppearanceInstance PlayerAppearance { get; private set; }
        #endregion

        public PlayerInstance(
            string id,
            string definitionId,
            ICollisionShape collisionShape,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 direction,
            CharacteristicInstance characteristic,
            InventoryInstance inventory,
            int level,
            List<EffectInstance> activeEffects,
            string userId,
            PlayerAppearanceInstance playerAppearance) : base(
                id,
                definitionId,
                collisionShape,
                roomSpatialId,
                layerZ,
                position,
                direction,
                playerAppearance,
                characteristic,
                inventory,
                level,
                activeEffects)
        {
            UserID = userId;
            PlayerAppearance = playerAppearance;
        }

        #region Methods
        public void UpdateAppearance(
            string skinId,
            HSV skinColor,
            string hairId,
            string eyesId,
            string shirtId,
            string pantId,
            HSV hairColor,
            HSV pantColor)
        {
            PlayerAppearance = new PlayerAppearanceInstance(
                skinId,
                skinColor,
                hairId,
                eyesId,
                shirtId,
                pantId,
                hairColor,
                pantColor
            );
        }
        #endregion
    }
}