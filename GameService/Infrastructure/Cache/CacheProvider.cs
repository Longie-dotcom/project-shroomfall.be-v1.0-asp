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
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Cache
{
    public class CacheProvider : ICacheProvider
    {
        #region Attributes
        private readonly ITelemetryQueue telemetryQueue;
        private readonly IServiceScopeFactory serviceScopeFactory;

        private readonly IAICache aiCache;
        private readonly IAppearanceCache appearanceCache;
        private readonly ICollisionCache collisionCache;
        private readonly ICharacteristicCache characteristicCache;
        private readonly IInteractableCache interactableCache;
        private readonly IInventoryCache inventoryCache;
        private readonly ILifetimeCache lifetimeCache;
        private readonly IPortalCache portalCache;
        private readonly IProjectileCache projectileCache;
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
        public IInteractableCache Interactable => interactableCache;
        public IInventoryCache Inventory => inventoryCache;
        public ILifetimeCache Lifetime => lifetimeCache;
        public IPortalCache Portal => portalCache;
        public IProjectileCache Projectile => projectileCache;
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
            IServiceScopeFactory serviceScopeFactory,

            IAICache aiCache,
            IAppearanceCache appearanceCache,
            ICollisionCache collisionCache,
            ICharacteristicCache characteristicCache,
            IInteractableCache interactableCache,
            IInventoryCache inventoryCache,
            ILifetimeCache lifetimeCache,
            IPortalCache portalCache,
            IProjectileCache projectileCache,
            ITriggeredEffectCache triggeredEffectCache,
            IEntityCache entityCache,
            ILocaleCache localeCache,
            IEffectCache effectCache,
            IItemCache itemCache,
            IRoomConnectionCache roomConnectionCache,
            IRoomCache roomCache)
        {
            this.telemetryQueue = telemetryQueue;
            this.serviceScopeFactory = serviceScopeFactory;

            this.aiCache = aiCache;
            this.appearanceCache = appearanceCache;
            this.collisionCache = collisionCache;
            this.characteristicCache = characteristicCache;
            this.interactableCache = interactableCache;
            this.inventoryCache = inventoryCache;
            this.lifetimeCache = lifetimeCache;
            this.portalCache = portalCache;
            this.projectileCache = projectileCache;
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
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var relationalUoW = scope.ServiceProvider.GetRequiredService<IRelationalUoW>();

                    // Resolve all repositories on demand from the Unit of Work
                    var aiRepository = relationalUoW.GetRepository<IAIDefinitionRepository>();
                    var appearanceRepository = relationalUoW.GetRepository<IAppearanceDefinitionRepository>();
                    var collisionRepository = relationalUoW.GetRepository<ICollisionDefinitionRepository>();
                    var characteristicRepository = relationalUoW.GetRepository<ICharacteristicDefinitionRepository>();
                    var interactableRepository = relationalUoW.GetRepository<IInteractableDefinitionRepository>();
                    var inventoryRepository = relationalUoW.GetRepository<IInventoryDefinitionRepository>();
                    var lifetimeRepository = relationalUoW.GetRepository<ILifetimeDefinitionRepository>();
                    var portalRepository = relationalUoW.GetRepository<IPortalDefinitionRepository>();
                    var projectileRepository = relationalUoW.GetRepository<IProjectileDefinitionRepository>();
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
                    interactableCache.Load((await interactableRepository.GetAllAsync()).ToList());
                    inventoryCache.Load((await inventoryRepository.GetAllAsync()).ToList());
                    lifetimeCache.Load((await lifetimeRepository.GetAllAsync()).ToList());
                    portalCache.Load((await portalRepository.GetAllAsync()).ToList());
                    projectileCache.Load((await projectileRepository.GetAllAsync()).ToList());
                    triggeredEffectCache.Load((await triggeredEffectRepository.GetAllAsync()).ToList());
                    entityCache.Load((await entityRepository.GetAllAsync()).ToList());
                    localeCache.Load((await localeRepository.GetAllAsync()).ToList());
                    effectCache.Load((await effectRepository.GetAllAsync()).ToList());
                    itemCache.Load((await itemRepository.GetAllAsync()).ToList());
                    roomConnectionCache.Load((await roomConnectionRepository.GetAllAsync()).ToList());
                    roomCache.Load((await roomRepository.GetAllAsync()).ToList());
                }

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