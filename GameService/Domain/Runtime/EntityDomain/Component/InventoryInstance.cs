using Contract.Enum.MetaDomain.Item;
using Domain.Abstraction;
using Domain.Runtime.MetaDomain;

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
        public ItemInstance? GetEquipped(
            EquipmentSlot slot)
        {
            return equippedCache.TryGetValue(slot, out var item) ? item : null;
        }

        public IReadOnlyDictionary<EquipmentSlot, ItemInstance> GetAllEquipped()
        {
            return equippedCache;
        }

        /// <summary>
        /// Equips an item into the specified slot.
        /// If an item was already equipped in that slot, it is unequipped and returned so the caller can publish sync events.
        /// </summary>
        /// <returns>The unequipped/swapped item, or null if the slot was empty.</returns>
        public ItemInstance? Equip(
            ItemInstance item,
            EquipmentSlot slot)
        {
            if (!Items.Contains(item))
                return null;

            ItemInstance? unequippedItem = null;

            // If a different item is already in this slot, untag and store reference
            if (equippedCache.TryGetValue(slot, out var existingItem))
            {
                existingItem.SetEquippedState(null);
                unequippedItem = existingItem;
            }

            // Tag the new item and update the cache
            item.SetEquippedState(slot);
            equippedCache[slot] = item;

            return unequippedItem;
        }

        /// <summary>
        /// Unequips the item from the target slot.
        /// </summary>
        /// <returns>The unequipped item instance, or null if nothing was equipped in that slot.</returns>
        public ItemInstance? Unequip(
            EquipmentSlot slot)
        {
            if (equippedCache.Remove(slot, out var item))
            {
                item.SetEquippedState(null);
                return item;
            }

            return null;
        }
        #endregion
    }
}