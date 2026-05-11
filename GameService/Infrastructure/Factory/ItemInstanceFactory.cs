using Application.Interfaces.Cache;
using Application.Interfaces.Factory;
using Domain.Definition.ItemDomain.Enum;
using Domain.Document.ItemDomain;
using Domain.DomainException;
using Domain.Runtime.ItemDomain;
using Domain.Shared;

namespace Infrastructure.Factory
{
    public class ItemInstanceFactory : IItemInstanceFactory
    {
        #region Attributes
        private readonly IItemCache itemCache;
        #endregion

        #region Properties
        #endregion

        public ItemInstanceFactory(
             IItemCache itemCache)
        {
            this.itemCache = itemCache;
        }

        #region Methods
        public ItemInstance Create(
            string definitionId,
            int count,
            int? currentDurability,
            ItemQuality quality)
        {
            var itemDef = itemCache.Get(definitionId);

            if (itemDef == null)
                throw new InternalException(
                    ResponseCode.ItemInstanceFactory_DefinitionNotFound,
                    $"Item definition with ID: {definitionId} is not found in cache");

            return new ItemInstance(
                id: Guid.NewGuid().ToString(),
                definitionId: itemDef.ID,
                count: count,
                durability: currentDurability ?? itemDef.Durability,
                quality: quality
            );
        }

        public ItemInstance CreateFromDocument(
            ItemDocument doc)
        {
            if (doc == null)
                throw new InternalException(
                    ResponseCode.ItemInstanceFactory_DocumentNotFound,
                    "Item document is null");

            var itemDef = itemCache.Get(doc.DefinitionID);

            if (itemDef == null)
                throw new InternalException(
                    ResponseCode.ItemInstanceFactory_DefinitionFromDocumentNotFound,
                    $"Item definition with ID: {doc.DefinitionID} is not found in cache");

            return new ItemInstance(
                id: doc.ID,
                definitionId: doc.DefinitionID,
                count: doc.Count,
                durability: doc.CurrentDurability,
                quality: doc.Quality
            );
        }
        #endregion
    }
}