using Domain.Common;
using Domain.Runtime.EntityDomain;

namespace Application.Services.Abstraction.ItemService
{
    public interface IItemService
    {
        void Use(
            CreatureInstance creature,
            string itemInstanceId,
            Vector2 objectPosition);
    }
}
