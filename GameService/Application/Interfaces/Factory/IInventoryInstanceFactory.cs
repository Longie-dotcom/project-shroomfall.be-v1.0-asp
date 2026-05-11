using Domain.Document.ItemDomain;
using Domain.Runtime.ItemDomain;

namespace Application.Interfaces.Factory
{
    public interface IInventoryInstanceFactory
    {
        InventoryInstance Create(
            string definitionId);
        InventoryInstance CreateFromDocument(
            InventoryDocument doc);
    }
}
