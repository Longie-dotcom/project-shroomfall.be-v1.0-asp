using Contract.Enum.EntityDomain;
using Contract.Enum.ItemDomain;

namespace Domain.Shared
{
    public static class EquipmentMapping
    {
        public static readonly Dictionary<ItemCategory, EquipmentSlot> Map = new()
        {
            { ItemCategory.Head, EquipmentSlot.Head },
            { ItemCategory.Chest, EquipmentSlot.Chest },
            { ItemCategory.Pant, EquipmentSlot.Pant },
            { ItemCategory.Shoe, EquipmentSlot.Shoe }
        };
    }
}