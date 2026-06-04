using Application.Interfaces.Cache;
using Application.Interfaces.Repository.Relational;

namespace Infrastructure.Cache
{
    public class CacheLoader : ICacheLoader
    {
        #region Attributes
        private IRelationalUoW relational;

        private readonly IAttributeValueCache attributeValueCache;
        private readonly ICharacteristicCache characteristicCache;
        private readonly IEffectCache effectCache;
        private readonly IEntityCache entityCache;
        private readonly IInventoryCache inventoryCache;
        private readonly IItemCache itemCache;
        private readonly ILocaleCache localeCache;
        private readonly IRoomConnectionCache roomConnectionCache;
        private readonly IRoomCache roomCache;
        #endregion

        #region Properties
        #endregion

        public CacheLoader(
             IRelationalUoW relational,

             IAttributeValueCache attributeValueCache,
             ICharacteristicCache characteristicCache,
             IEffectCache effectCache,
             IEntityCache entityCache,
             IInventoryCache inventoryCache,
             IItemCache itemCache,
             ILocaleCache localeCache,
             IRoomConnectionCache roomConnectionCache,
             IRoomCache roomCache)
        {
            this.relational = relational;

            this.attributeValueCache = attributeValueCache;
            this.characteristicCache = characteristicCache;
            this.effectCache = effectCache;
            this.entityCache = entityCache;
            this.inventoryCache = inventoryCache;
            this.itemCache = itemCache;
            this.localeCache = localeCache;
            this.roomConnectionCache = roomConnectionCache;
            this.roomCache = roomCache;
        }

        #region Methods
        public async Task LoadAllAsync()
        {
            var attributeValues = await relational
                .GetRepository<IAttributeValueRepository>()
                .GetAllAsync();

            var characteristics = await relational
                .GetRepository<ICharacteristicRepository>()
                .GetAllWithAttributeValuesAsync();

            var effects = await relational
                .GetRepository<IEffectRepository>()
                .GetAllAsync();

            var entities = await relational
                .GetRepository<IEntityRepository>()
                .GetAllAsync();

            var inventories = await relational
                .GetRepository<IInventoryRepository>()
                .GetAllWithDefaultItemsAsync();

            var items = await relational
                .GetRepository<IItemRepository>()
                .GetAllWithEffectsAsync();

            var locales = await relational
                .GetRepository<ILocaleRepository>()
                .GetAllWithLocalizationEntriesAsync();

            var roomConnections = await relational
                .GetRepository<IRoomConnectionRepository>()
                .GetAllAsync();

            var rooms = await relational
                .GetRepository<IRoomRepository>()
                .GetAllWithCellsAndSpawnRulesAsync();

            attributeValueCache.Load(attributeValues);
            characteristicCache.Load(characteristics);
            effectCache.Load(effects);
            entityCache.Load(entities);
            inventoryCache.Load(inventories);
            itemCache.Load(items);
            localeCache.Load(locales);
            roomConnectionCache.Load(roomConnections);
            roomCache.Load(rooms);
        }
        #endregion
    }
}