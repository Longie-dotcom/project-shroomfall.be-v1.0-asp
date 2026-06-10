using Application.Interfaces.Cache;
using AutoMapper;
using Contract.DTO.Definition;
using Contract.DTO.Design;
using Domain.Shared;

namespace Application.Services.DesignService
{
    public class BuilderService
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly IAttributeValueCache attributeValueCache;
        private readonly ICharacteristicCache characteristicCache;
        private readonly IEffectCache effectCache;
        private readonly IItemCache itemCache;
        private readonly IInventoryCache inventoryCache;
        private readonly IEntityCache entityCache;
        private readonly IRoomCache roomCache;
        private readonly IRoomConnectionCache roomConnectionCache;
        private readonly ILocaleCache localeCache;
        #endregion

        #region Properties
        #endregion

        public BuilderService(
            IMapper mapper,
            IAttributeValueCache attributeValueCache,
            ICharacteristicCache characteristicCache,
            IEffectCache effectCache,
            IItemCache itemCache,
            IInventoryCache inventoryCache,
            IEntityCache entityCache,
            IRoomCache roomCache,
            IRoomConnectionCache roomConnectionCache,
            ILocaleCache localeCache)
        {
            this.mapper = mapper;
            this.attributeValueCache = attributeValueCache;
            this.characteristicCache = characteristicCache;
            this.effectCache = effectCache;
            this.itemCache = itemCache;
            this.inventoryCache = inventoryCache;
            this.entityCache = entityCache;
            this.roomCache = roomCache;
            this.roomConnectionCache = roomConnectionCache;
            this.localeCache = localeCache;
        }

        #region Methods
        public DefinitionSnapshotDTO BuildDefinitionSnapshot(long version)
        {
            // ─────────────────────────────
            // Base cached data
            // ─────────────────────────────
            var attributeDefs = AttributeDefinitions.AllList();
            var attributeValues = attributeValueCache.GetAll();
            var characteristics = characteristicCache.GetAll();
            var effects = effectCache.GetAll();
            var items = itemCache.GetAll();
            var inventories = inventoryCache.GetAll();
            var entities = entityCache.GetAll();
            var rooms = roomCache.GetAll();
            var roomConnections = roomConnectionCache.GetAll();
            var locales = localeCache.GetAll();

            return new DefinitionSnapshotDTO
            {
                Version = version,

                // ─────────────────────────────
                // Attribute domain
                // ─────────────────────────────
                AttributeDefinitions = attributeDefs
                    .Select(x => mapper.Map<AttributeDefinitionDTO>(x))
                    .ToList(),

                AttributeValues = attributeValues
                    .Select(x => mapper.Map<AttributeValueDefinitionDTO>(x))
                    .ToList(),

                Characteristics = characteristics
                    .Select(x => mapper.Map<CharacteristicDefinitionDTO>(x))
                    .ToList(),

                Effects = effects
                    .Select(x => mapper.Map<EffectDefinitionDTO>(x))
                    .ToList(),

                // ─────────────────────────────
                // Item domain
                // ─────────────────────────────
                InventoryItems = inventories
                    .SelectMany(i => i.DefaultItems)
                    .Select(x => mapper.Map<InventoryItemDefinitionDTO>(x))
                    .ToList(),

                Inventories = inventories
                    .Select(x => mapper.Map<InventoryDefinitionDTO>(x))
                    .ToList(),

                Items = items
                    .Select(x => mapper.Map<ItemDefinitionDTO>(x))
                    .ToList(),

                ItemEffects = items
                    .SelectMany(i => i.Effects)
                    .Select(e => mapper.Map<ItemEffectDefinitionDTO>(e))
                    .ToList(),

                // ─────────────────────────────
                // Entity domain
                // ─────────────────────────────
                Entities = entities
                    .Select(x => mapper.Map<EntityDefinitionDTO>(x))
                    .ToList(),

                // ─────────────────────────────
                // World domain
                // ─────────────────────────────
                Cells = rooms
                    .SelectMany(r => r.Cells)
                    .Select(c => mapper.Map<CellDefinitionDTO>(c))
                    .ToList(),

                EntitySpawnRules = rooms
                    .SelectMany(r => r.EntitySpawnRules)
                    .Select(x => mapper.Map<EntitySpawnRuleDefinitionDTO>(x))
                    .ToList(),

                Rooms = rooms
                    .Select(x => mapper.Map<RoomDefinitionDTO>(x))
                    .ToList(),

                RoomConnections = roomConnections
                    .Select(x => mapper.Map<RoomConnectionDefinitionDTO>(x))
                    .ToList(),

                SpawnAreas = rooms
                    .SelectMany(r => r.EntitySpawnRules)
                    .SelectMany(e => e.SpawnAreas)
                    .Select(x => mapper.Map<SpawnAreaDefinitionDTO>(x))
                    .ToList(),

                // ─────────────────────────────
                // Localization
                // ─────────────────────────────
                Locales = locales
                    .Select(x => mapper.Map<LocaleDTO>(x))
                    .ToList(),
            };
        }
        #endregion
    }
}