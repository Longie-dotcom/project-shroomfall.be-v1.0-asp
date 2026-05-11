using Domain.Definition.ItemDomain.Enum;
using Domain.Definition.LocalizationDomain;
using Domain.DomainException;
using Domain.Shared;

namespace Domain.Definition.ItemDomain
{
    public class Inventory
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; }
        public InventoryType Type { get; private set; }
        public LocalizedText LocalizedText { get; private set; }
        public int SlotCount { get; private set; }

        public ICollection<InventoryItem> DefaultItems { get; private set; } = new List<InventoryItem>();
        #endregion

        protected Inventory() 
        { 
        
        }

        public Inventory(
            string id,
            InventoryType type,
            LocalizedText localizedText,
            int slotCount)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadRequest(ResponseCode.Inventory_InvalidId);

            if (string.IsNullOrWhiteSpace(localizedText.NameKey))
                throw new BadRequest(ResponseCode.Inventory_InvalidName);

            if (string.IsNullOrWhiteSpace(localizedText.DescriptionKey))
                throw new BadRequest(ResponseCode.Inventory_InvalidDescription);

            if (slotCount < 0)
                throw new BadRequest(ResponseCode.Inventory_InvalidSlotCount);

            ID = id;
            Type = type;
            LocalizedText = localizedText;
            SlotCount = slotCount;
        }

        #region Methods
        #endregion
    }
}