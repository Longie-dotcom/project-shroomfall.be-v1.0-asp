using Domain.Common;
using Domain.Definition.ItemDomain;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.ItemDomain;

namespace Application.Services.Abstraction.ItemService
{
    public interface IPlacementService
    {
        void Place(
            CreatureInstance creature,
            ItemInstance item,
            Item itemDef,
            Vector2 position);
    }
}
