using Domain.Definition.ItemDomain.Enum;
using Domain.Document.ItemDomain;
using Domain.Runtime.ItemDomain;

namespace Application.Interfaces.Factory
{
    public interface IItemInstanceFactory
    {
        ItemInstance Create(
            string definitionId,
            int count,
            int? currentDurability,
            ItemQuality quality);
        ItemInstance CreateFromDocument(
            ItemDocument doc);
    }
}
