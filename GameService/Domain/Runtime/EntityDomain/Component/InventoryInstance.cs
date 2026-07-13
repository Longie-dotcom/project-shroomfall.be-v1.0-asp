using Contract.Enum.MetaDomain.Item;
using Domain.Abstraction;
using Domain.DomainException;
using Domain.Runtime.MetaDomain;
using ResponseCode;

namespace Domain.Runtime.EntityDomain.Component
{
    public class InventoryInstance : ComponentInstance
    {
        #region Attributes
        private readonly Dictionary<EquipmentSlot, ItemInstance> equippedCache = new();
        #endregion

        #region Properties
        public List<ItemInstance> Items { get; }
        #endregion

        public InventoryInstance(
            Guid definitionId,
            List<ItemInstance> items) : base(definitionId)
        {
            Items = items;

            foreach (var item in items.Where(i => i.IsEquipped()))
            {
                equippedCache[item.EquippedSlot!.Value] = item;
            }
        }

        #region Methods
        public void AddItems(
            List<ItemInstance> items)
        {
            Items.AddRange(items);
        }

        public ItemInstance? GetEquipped(
            EquipmentSlot slot)
        {
            return equippedCache.TryGetValue(slot, out var item) ? item : null;
        }

        public IReadOnlyDictionary<EquipmentSlot, ItemInstance> GetAllEquipped()
        {
            return equippedCache;
        }

        public void Equip(
            ItemInstance item, 
            EquipmentSlot slot)
        {
            if (!Items.Contains(item))
                throw new InternalException(
                    DomainCode.InventoryInstanceCode.EquippedItemNotExistInInventory,
                    $"Item equipped process failed, inventory instance of entity id: {Entity.ID} has no such item [{item.ID}][{item.DefinitionID}]");

            // If a different item is already in this slot, it must be forcefully un-tagged first
            if (equippedCache.TryGetValue(slot, out var existingItem))
            {
                existingItem.SetEquippedState(null);
            }

            // Tag the new item and update the cache
            item.SetEquippedState(slot);
            equippedCache[slot] = item;
        }

        public void Unequip(
            EquipmentSlot slot)
        {
            if (equippedCache.Remove(slot, out var item))
            {
                item.SetEquippedState(null);
            }
        }
        #endregion
    }
}