using Domain.Definition.ItemDomain.Enum;
using Domain.DomainException;
using Domain.Shared;

namespace Domain.Definition.ItemDomain
{
    public class InventoryItem
    {
        #region Attributes
        #endregion

        #region Properties
        public string InventoryID { get; private set; }
        public string ItemID { get; private set; }
        public int Amount { get; private set; }
        public ItemQuality Quality { get; private set; }

        public Inventory Inventory { get; private set; }
        public Item Item { get; private set; }
        #endregion

        protected InventoryItem() 
        {
        
        }

        public InventoryItem(
            string inventoryId,
            string itemId,
            int amount,
            ItemQuality quality)
        {
            if (string.IsNullOrWhiteSpace(inventoryId))
                throw new BadRequest(ResponseCode.InventoryItem_InvalidInventoryId);

            if (string.IsNullOrWhiteSpace(itemId))
                throw new BadRequest(ResponseCode.InventoryItem_InvalidItemId);

            if (amount < 0 || amount > Constraint.MAX_ITEM_AMOUNT_PER_SLOT)
                throw new BadRequest(ResponseCode.InventoryItem_InvalidAmount);

            InventoryID = inventoryId;
            ItemID = itemId;
            Amount = amount;
            Quality = quality;
        }

        #region Methods
        #endregion
    }
}