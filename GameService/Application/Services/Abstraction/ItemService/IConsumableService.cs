using Domain.Definition.ItemDomain;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.ItemDomain;

namespace Application.Services.Abstraction.ItemService
{
    public interface IConsumableService
    {
        void Consume(
            CreatureInstance creature,
            ItemInstance item,
            Item itemDef);
    }
}
