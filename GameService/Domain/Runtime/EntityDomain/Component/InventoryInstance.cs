using Contract.Enum.MetaDomain.Item;
using Domain.Abstraction;
using Domain.DomainException;
using ResponseCode;

namespace Domain.Runtime.EntityDomain.Component
{
    public class InventoryInstance : ComponentInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public List<ItemInstance> Items { get; }
        #endregion

        public InventoryInstance(
            Guid definitionId,
            List<ItemInstance> items) : base(definitionId)
        {
            Items = items;
        }

        #region Methods
        public void AddItems(
            List<ItemInstance> items)
        {
            Items.AddRange(items);
        }
        #endregion
    }

    public class ItemInstance : IItemStateContract
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; } = string.Empty;
        public string DefinitionID { get; }
        public int Amount { get; private set; }
        public ItemQuality Quality { get; private set; }
        public int? Durability { get; private set; }
        #endregion

        public ItemInstance(
            string id,
            string definitionId,
            int amount,
            ItemQuality quality,
            int? durability = null)
        {
            ID = id;
            DefinitionID = definitionId;
            Amount = amount;
            Quality = quality;
            Durability = durability;
        }

        #region Methods
        public void AddAmount(
            int amountToAdd)
        {
            if (amountToAdd <= 0)
                throw new BadRequest(
                    DomainCode.InventoryInstanceCode.AddAmountInvalid,
                    $"Item mutation failed for instance '{ID}' (Def: '{DefinitionID}'). Amount to add must be greater than zero. Received: {amountToAdd}");

            Amount += amountToAdd;
        }

        public void RemoveAmount(
            int amountToRemove)
        {
            if (amountToRemove <= 0)
                throw new BadRequest(
                    DomainCode.InventoryInstanceCode.RemoveAmountInvalid,
                    $"Item mutation failed for instance '{ID}' (Def: '{DefinitionID}'). Amount to remove must be greater than zero. Received: {amountToRemove}");

            if (Amount < amountToRemove)
                throw new BadRequest(
                    DomainCode.InventoryInstanceCode.InsufficientItemAmount,
                    $"Item mutation failed for instance '{ID}' (Def: '{DefinitionID}'). Cannot remove {amountToRemove} units. Only {Amount} available.");

            Amount -= amountToRemove;
        }

        public bool DegradeDurability(
            int amountToDegrade)
        {
            if (!Durability.HasValue) return false;

            Durability = Math.Max(0, Durability.Value - amountToDegrade);
            return Durability.Value <= 0;
        }
        #endregion
    }
}