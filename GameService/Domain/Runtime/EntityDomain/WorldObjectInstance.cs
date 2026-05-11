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
        public string? RoomSpatialReferenceID { get; private set; } = string.Empty;
        #endregion

        public WorldObjectInstance(
            string id,
            string definitionId,
            ICollisionShape collisionShape,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 direction,
            AppearanceInstance appearance,
            InventoryInstance? inventory,
            string? roomSpatialReferenceId) : base(
                id,
                definitionId,
                collisionShape,
                roomSpatialId,
                layerZ,
                position,
                direction,
                appearance)
        {
            Inventory = inventory;
            RoomSpatialReferenceID = roomSpatialReferenceId;
        }

        #region Methods
        #endregion
    }
}