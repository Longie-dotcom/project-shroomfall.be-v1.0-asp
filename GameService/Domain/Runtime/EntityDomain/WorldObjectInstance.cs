using Domain.Abstraction;
using Domain.Common;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.ItemDomain;

namespace Domain.Runtime.EntityDomain
{
    public class WorldObjectInstance : EntityInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public InventoryInstance? Inventory { get; private set; }
        #endregion

        public WorldObjectInstance(
            string id,
            string definitionId,
            ICollisionShape collisionShape,
            Vector2 collisionOffset,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 movementVector,
            AppearanceInstance appearance,
            InventoryInstance? inventory) : base(
                id,
                definitionId,
                collisionShape,
                collisionOffset,
                roomSpatialId,
                layerZ,
                position,
                movementVector,
                appearance)
        {
            Inventory = inventory;
        }

        #region Methods
        #endregion
    }
}