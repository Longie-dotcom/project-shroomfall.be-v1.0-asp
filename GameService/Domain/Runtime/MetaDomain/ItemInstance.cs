using Contract;
using Contract.Enum.MetaDomain.Item;
using Domain.DomainException;
using ResponseCode;

namespace Domain.Runtime.MetaDomain
{
    public class ItemInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; } = string.Empty;
        public string DefinitionID { get; }
        public int Amount { get; private set; }
        public ItemQuality Quality { get; private set; }
        public int? Durability { get; private set; }
        public EquipmentSlot? EquippedSlot { get; private set; }
        #endregion

        public ItemInstance(
            string id,
            string definitionId,
            int amount,
            ItemQuality quality,
            int? durability = null,
            EquipmentSlot? equippedSlot = null)
        {
            ID = id;
            DefinitionID = definitionId;
            Amount = amount;
            Quality = quality;
            Durability = durability;
            EquippedSlot = equippedSlot;
        }

        #region Methods
        public void AddAmount(
            int amountToAdd)
        {
            if (amountToAdd <= 0)
                throw new BadRequest(
                    DomainCode.ItemInstanceCode.AddAmountInvalid,
                    $"Item mutation failed for instance '{ID}' (Def: '{DefinitionID}'). Amount to add must be greater than zero. Received: {amountToAdd}");

            Amount += amountToAdd;
        }

        public void RemoveAmount(
            int amountToRemove)
        {
            if (amountToRemove <= 0)
                throw new BadRequest(
                    DomainCode.ItemInstanceCode.RemoveAmountInvalid,
                    $"Item mutation failed for instance '{ID}' (Def: '{DefinitionID}'). Amount to remove must be greater than zero. Received: {amountToRemove}");

            if (Amount < amountToRemove)
                throw new BadRequest(
                    DomainCode.ItemInstanceCode.InsufficientItemAmount,
                    $"Item mutation failed for instance '{ID}' (Def: '{DefinitionID}'). Cannot remove {amountToRemove} units. Only {Amount} available.");

            Amount -= amountToRemove;
        }

        public bool DegradeDurability()
        {
            if (!Durability.HasValue) return false;

            Durability = Math.Max(0, Durability.Value - Constraint.ITEM_DEGRADED_VALUE);
            return Durability.Value <= 0;
        }

        public void SetEquippedState(
            EquipmentSlot? slot)
        {
            EquippedSlot = slot;
        }

        public bool IsEquipped()
        {
            return EquippedSlot.HasValue;
        }
        #endregion
    }
}