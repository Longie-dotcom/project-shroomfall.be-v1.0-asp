using Application.Interfaces.Cache;
using Application.Interfaces.Factory;
using Domain.Document.ItemDomain;
using Domain.DomainException;
using Domain.Runtime.ItemDomain;
using Domain.Shared;

namespace Infrastructure.Factory
{
    public class InventoryInstanceFactory : IInventoryInstanceFactory
    {
        #region Attributes
        private readonly IInventoryCache inventoryCache;
        private readonly IItemInstanceFactory itemInstanceFactory;
        #endregion

        #region Properties
        #endregion

        public InventoryInstanceFactory(
            IInventoryCache inventoryCache, 
            IItemInstanceFactory itemInstanceFactory)
        {
            this.inventoryCache = inventoryCache;
            this.itemInstanceFactory = itemInstanceFactory;
        }

        #region Methods
        public InventoryInstance Create(
            string definitionId)
        {
            var inventoryDef = inventoryCache.Get(definitionId);

            if (inventoryDef == null)
                throw new InternalException(
                    ResponseCode.InventoryInstanceFactory_DefinitionNotFound,
                    $"Inventory definition with ID: {definitionId} is not found in cache");

            var items = inventoryDef.DefaultItems.Select(defSlot =>
                new ItemInstance(
                    id: Guid.NewGuid().ToString(),
                    definitionId: defSlot.Item.ID,
                    count: defSlot.Amount,
                    durability: defSlot.Item.Durability,
                    quality: defSlot.Quality
                )).ToList();

            return new InventoryInstance(
                id: Guid.NewGuid().ToString(),
                definitionID: inventoryDef.ID,
                items: items
            );
        }

        public InventoryInstance CreateFromDocument(
            InventoryDocument doc)
        {
            if (doc == null)
                throw new InternalException(
                    ResponseCode.InventoryInstanceFactory_DocumentNotFound,
                    "Inventory document is null");

            var inventoryDef = inventoryCache.Get(doc.DefinitionID);

            if (inventoryDef == null)
                throw new InternalException(
                    ResponseCode.InventoryInstanceFactory_DefinitionFromDocumentNotFound,
                    $"Inventory definition with ID: {doc.DefinitionID} is not found in cache");

            var items = doc.Items.Select(itemDoc =>
                    itemInstanceFactory.CreateFromDocument(itemDoc)
                ).ToList();

            return new InventoryInstance(
                id: doc.ID,
                definitionID: doc.DefinitionID,
                items: items
            );
        }
        #endregion
    }
}