using Application.Interfaces.Cache;
using Application.Interfaces.Cache.EntityDomain;
using Application.Interfaces.Cache.EntityDomain.Component;
using Application.Interfaces.Cache.LocalizationDomain;
using Application.Interfaces.Cache.MetaDomain;
using Application.Interfaces.Cache.WorldDomain;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Application.Interfaces.Utility;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Infrastructure.Cache
{
    public class CacheProvider : ICacheProvider
    {
        #region Attributes
        private readonly ITelemetryQueue telemetryQueue;

        private readonly IRelationalUoW relationalUoW;

        private readonly IAICache aiCache;
        private readonly IAppearanceCache appearanceCache;
        private readonly ICollisionCache collisionCache;
        private readonly ICharacteristicCache characteristicCache;
        private readonly IEntityRelationshipCache entityRelationshipCache;
        private readonly IInteractableCache interactableCache;
        private readonly IInventoryCache inventoryCache;
        private readonly ILifetimeCache lifetimeCache;
        private readonly IProjectileCache projectileCache;
        private readonly ISpawnCache spawnCache;
        private readonly ITriggeredEffectCache triggeredEffectCache;
        private readonly IEntityCache entityCache;
        private readonly ILocaleCache localeCache;
        private readonly IEffectCache effectCache;
        private readonly IItemCache itemCache;
        private readonly IRoomConnectionCache roomConnectionCache;
        private readonly IRoomCache roomCache;
        #endregion

        #region Properties
        public IAICache AI => aiCache;
        public IAppearanceCache Appearance => appearanceCache;
        public ICollisionCache Collision => collisionCache;
        public ICharacteristicCache Characteristic => characteristicCache;
        public IEntityRelationshipCache EntityRelationship => entityRelationshipCache;
        public IInteractableCache Interactable => interactableCache;
        public IInventoryCache Inventory => inventoryCache;
        public ILifetimeCache Lifetime => lifetimeCache;
        public IProjectileCache Projectile => projectileCache;
        public ISpawnCache Spawn => spawnCache;
        public ITriggeredEffectCache TriggeredEffect => triggeredEffectCache;
        public IEntityCache Entity => entityCache;
        public ILocaleCache Locale => localeCache;
        public IEffectCache Effect => effectCache;
        public IItemCache Item => itemCache;
        public IRoomConnectionCache RoomConnection => roomConnectionCache;
        public IRoomCache Room => roomCache;
        #endregion

        public CacheProvider(
            ITelemetryQueue telemetryQueue,

            IRelationalUoW relationalUoW,

            IAICache aiCache,
            IAppearanceCache appearanceCache,
            ICollisionCache collisionCache,
            ICharacteristicCache characteristicCache,
            IEntityRelationshipCache entityRelationshipCache,
            IInteractableCache interactableCache,
            IInventoryCache inventoryCache,
            ILifetimeCache lifetimeCache,
            IProjectileCache projectileCache,
            ISpawnCache spawnCache,
            ITriggeredEffectCache triggeredEffectCache,
            IEntityCache entityCache,
            ILocaleCache localeCache,
            IEffectCache effectCache,
            IItemCache itemCache,
            IRoomConnectionCache roomConnectionCache,
            IRoomCache roomCache)
        {
            this.telemetryQueue = telemetryQueue;

            this.relationalUoW = relationalUoW;

            this.aiCache = aiCache;
            this.appearanceCache = appearanceCache;
            this.collisionCache = collisionCache;
            this.characteristicCache = characteristicCache;
            this.entityRelationshipCache = entityRelationshipCache;
            this.interactableCache = interactableCache;
            this.inventoryCache = inventoryCache;
            this.lifetimeCache = lifetimeCache;
            this.projectileCache = projectileCache;
            this.spawnCache = spawnCache;
            this.triggeredEffectCache = triggeredEffectCache;
            this.entityCache = entityCache;
            this.localeCache = localeCache;
            this.effectCache = effectCache;
            this.itemCache = itemCache;
            this.roomConnectionCache = roomConnectionCache;
            this.roomCache = roomCache;
        }

        #region Methods
        public async Task LoadAllAsync()
        {
            try
            {
                // Resolve all repositories on demand from the Unit of Work
                var aiRepository = relationalUoW.GetRepository<IAIDefinitionRepository>();
                var appearanceRepository = relationalUoW.GetRepository<IAppearanceDefinitionRepository>();
                var collisionRepository = relationalUoW.GetRepository<ICollisionDefinitionRepository>();
                var characteristicRepository = relationalUoW.GetRepository<ICharacteristicDefinitionRepository>();
                var entityRelationshipDefinitionRepository = relationalUoW.GetRepository<IEntityRelationshipDefinitionRepository>();
                var interactableRepository = relationalUoW.GetRepository<IInteractableDefinitionRepository>();
                var inventoryRepository = relationalUoW.GetRepository<IInventoryDefinitionRepository>();
                var lifetimeRepository = relationalUoW.GetRepository<ILifetimeDefinitionRepository>();
                var projectileRepository = relationalUoW.GetRepository<IProjectileDefinitionRepository>();
                var spawnRepository = relationalUoW.GetRepository<ISpawnDefinitionRepository>();
                var triggeredEffectRepository = relationalUoW.GetRepository<ITriggeredEffectDefinitionRepository>();
                var entityRepository = relationalUoW.GetRepository<IEntityDefinitionRepository>();
                var localeRepository = relationalUoW.GetRepository<ILocaleRepository>();
                var effectRepository = relationalUoW.GetRepository<IEffectDefinitionRepository>();
                var itemRepository = relationalUoW.GetRepository<IItemDefinitionRepository>();
                var roomConnectionRepository = relationalUoW.GetRepository<IRoomConnectionRepository>();
                var roomRepository = relationalUoW.GetRepository<IRoomDefinitionRepository>();

                // Hydrate caches
                aiCache.Load((await aiRepository.GetAllAsync()).ToList());
                appearanceCache.Load((await appearanceRepository.GetAllAsync()).ToList());
                collisionCache.Load((await collisionRepository.GetAllAsync()).ToList());
                characteristicCache.Load((await characteristicRepository.GetAllAsync()).ToList());
                entityRelationshipCache.Load((await entityRelationshipDefinitionRepository.GetAllAsync()).ToList());
                interactableCache.Load((await interactableRepository.GetAllAsync()).ToList());
                inventoryCache.Load((await inventoryRepository.GetAllAsync()).ToList());
                lifetimeCache.Load((await lifetimeRepository.GetAllAsync()).ToList());
                projectileCache.Load((await projectileRepository.GetAllAsync()).ToList());
                spawnCache.Load((await spawnRepository.GetAllAsync()).ToList());
                triggeredEffectCache.Load((await triggeredEffectRepository.GetAllAsync()).ToList());
                entityCache.Load((await entityRepository.GetAllAsync()).ToList());
                localeCache.Load((await localeRepository.GetAllAsync()).ToList());
                effectCache.Load((await effectRepository.GetAllAsync()).ToList());
                itemCache.Load((await itemRepository.GetAllAsync()).ToList());
                roomConnectionCache.Load((await roomConnectionRepository.GetAllAsync()).ToList());
                roomCache.Load((await roomRepository.GetAllAsync()).ToList());

                telemetryQueue.EnqueueAlert(
                    InfrastructureCode.CacheProviderCode.LoadSuccess,
                    "Global caches successfully hydrated with metadata definitions.",
                    TelemetrySeverity.Info);
            }
            catch (Exception ex) when (ex is not InternalException)
            {
                throw new InternalException(
                    InfrastructureCode.CacheProviderCode.LoadFailed,
                    $"Critical failure occurred during global cache hydration (LoadAllAsync): {ex.Message}");
            }
        }
        #endregion
    }
}