using Contract.Enum.MetaDomain.Item;
using Domain.Abstraction;

namespace Domain.Definition.EntityDomain.Component
{
    public class InventoryDefinition : ComponentDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public int SlotCount { get; private set; }

        public List<InventoryEntry> DefaultItems { get; private set; } = new();
        #endregion

        protected InventoryDefinition() : base() { }

        public InventoryDefinition(
            Guid id,
            string entityDefinitionId,
            int slotCount) : base(id, entityDefinitionId)
        {
            SlotCount = slotCount;
        }

        #region Methods
        #endregion
    }

    public class InventoryEntry : IItemStateContract
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid ID { get; private set; }
        public string DefinitionID { get; private set; } = string.Empty;
        public int Amount { get; private set; }
        public ItemQuality Quality { get; private set; }
        public int? Durability => null;
        
        public Guid InventoryDefinitionID { get; private set; }
        public InventoryDefinition InventoryDefinition { get; private set; }
        #endregion

        protected InventoryEntry() { }

        public InventoryEntry(
            Guid id,
            string definitionId,
            int amount,
            ItemQuality quality,
            Guid inventoryDefinitionId)
        {
            ID = id;
            DefinitionID = definitionId;
            Amount = amount;
            Quality = quality;
            InventoryDefinitionID = inventoryDefinitionId;
        }

        #region Methods
        #endregion
    }
}