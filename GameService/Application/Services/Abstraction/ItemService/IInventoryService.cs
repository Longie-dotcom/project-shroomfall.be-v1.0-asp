using Domain.Runtime.EntityDomain;
using Domain.Runtime.ItemDomain;

namespace Application.Services.Abstraction.ItemService
{
    public interface IInventoryService
    {
        ItemInstance? AddItem(
            CreatureInstance creature,
            ItemInstance item);
        ItemInstance RemoveForEquip(
            CreatureInstance creature,
            string itemInstanceId);
        ItemInstance RemoveForConsume(
            CreatureInstance creature,
            string itemInstanceId);
        bool CanAddItem(
            CreatureInstance creature,
            ItemInstance item);
    }
}
