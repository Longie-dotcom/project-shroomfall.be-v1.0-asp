using Contract.Enum.EntityDomain;
using Domain.Definition.EntityDomain.Component;
using Domain.Definition.LocalizationDomain;

namespace Domain.Definition.EntityDomain
{
    public class WorldObject : Entity
    {
        #region Attributes
        public WorldObjectInteractionType InteractionType { get; private set; }
        public bool IsInteractable { get; private set; }
        public bool IsPickupable { get; private set; }
        public string? InventoryID { get; private set; }
        #endregion

        #region Properties
        #endregion

        protected WorldObject()
        {

        }

        public WorldObject(
            string id,
            EntityType type,
            LocalizedText localizedText,
            Appearance appearance,
            Collision collision,
            WorldObjectInteractionType interactionType,
            bool isInteractable,
            bool isPickupable,
            string? inventoryId) : base(
                id,
                type,
                localizedText,
                appearance,
                collision)
        {
            InteractionType = interactionType;
            IsInteractable = isInteractable;
            IsPickupable = isPickupable;
            InventoryID = inventoryId;
        }

        #region Methods
        #endregion
    }
}
