using Contract.Enum.MetaDomain.Item;
using Domain.Abstraction;

namespace Domain.Runtime.EntityDomain.Component
{
    public class EquipmentInstance : ComponentInstance
    {
        #region Attributes
        private readonly Dictionary<EquipmentSlot, ItemInstance?> slots = new();
        #endregion

        #region Properties
        public IReadOnlyDictionary<EquipmentSlot, ItemInstance?> Slots => slots;
        #endregion

        public EquipmentInstance() : base(Guid.Empty)
        {
            foreach (EquipmentSlot slot in Enum.GetValues<EquipmentSlot>())
            {
                slots[slot] = null;
            }
        }

        #region Methods
        public bool Equip(
            EquipmentSlot slot,
            ItemInstance item)
        {
            // Note: Add verification checks here (e.g., Level requirements, Item category match)

            slots[slot] = item;

            return true;
        }

        public ItemInstance? Unequip(
            EquipmentSlot slot)
        {
            var item = slots[slot];
            slots[slot] = null;
            return item;
        }

        public void LoadSlot(
            EquipmentSlot slot,
            ItemInstance? item)
        {
            slots[slot] = item;
        }
        #endregion
    }
}