using Domain.Definition.ItemDomain;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Enum;
using Domain.Runtime.ItemDomain;

namespace Application.Services.Abstraction.ItemService
{
    public interface IEquipmentService
    {
        void Equip(
            CreatureInstance creature,
            ItemInstance item,
            Item itemDef);
        void Unequip(
            CreatureInstance creature,
            EquipmentSlot slot);
        void RehydrateEquipment(
            CreatureInstance creature,
            Dictionary<EquipmentSlot, ItemInstance?> equipment);
    }
}
