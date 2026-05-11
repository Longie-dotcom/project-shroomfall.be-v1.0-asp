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
        private readonly IRoomCache roomCache;
        private readonly ITileCache tileCache;
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
             IRoomCache roomCache,
             ITileCache tileCache)
        {
            this.relational = relational;

            this.attributeValueCache = attributeValueCache;
            this.characteristicCache = characteristicCache;
            this.effectCache = effectCache;
            this.entityCache = entityCache;
            this.inventoryCache = inventoryCache;
            this.itemCache = itemCache;
            this.localeCache = localeCache;
            this.roomCache = roomCache;
            this.tileCache = tileCache;
        }

        #region Methods
        public async Task LoadAllAsync()
        {
            var attributeValuesTask =
                relational.GetRepository<IAttributeValueRepository>().GetAllAsync();
            var characteristicsTask = 
                relational.GetRepository<ICharacteristicRepository>().GetAllWithAttributeValuesAsync();
            var effectsTask = 
                relational.GetRepository<IEffectRepository>().GetAllAsync();
            var entitiesTask = 
                relational.GetRepository<IEntityRepository>().GetAllAsync();
            var inventoriesTask = 
                relational.GetRepository<IInventoryRepository>().GetAllWithDefaultItemsAsync();
            var itemsTask = 
                relational.GetRepository<IItemRepository>().GetAllWithEffectsAsync();
            var localeTask = 
                relational.GetRepository<ILocaleRepository>().GetAllWithLocalizationEntriesAsync();
            var roomsTask = 
                relational.GetRepository<IRoomRepository>().GetAllWithCellsAndSpawnRulesAsync();
            var tilesTask = 
                relational.GetRepository<ITileRepository>().GetAllAsync();

            await Task.WhenAll(
                attributeValuesTask,
                characteristicsTask,
                effectsTask,
                entitiesTask,
                inventoriesTask,
                itemsTask,
                localeTask,
                roomsTask,
                tilesTask
            );

            attributeValueCache.Load(await attributeValuesTask);
            characteristicCache.Load(await characteristicsTask);
            effectCache.Load(await effectsTask);
            entityCache.Load(await entitiesTask);
            inventoryCache.Load(await inventoriesTask);
            itemCache.Load(await itemsTask);
            localeCache.Load(await localeTask);
            roomCache.Load(await roomsTask);
            tileCache.Load(await tilesTask);
        }
        #endregion
    }
}