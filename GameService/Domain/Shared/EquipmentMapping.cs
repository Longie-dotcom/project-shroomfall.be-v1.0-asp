using Domain.Definition.ItemDomain.Enum;
using Domain.Runtime.EntityDomain.Enum;

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