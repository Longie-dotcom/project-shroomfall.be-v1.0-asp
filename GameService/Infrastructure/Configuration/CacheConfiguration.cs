using Application.Interfaces.Cache;
using Application.Interfaces.Cache.EntityDomain;
using Application.Interfaces.Cache.EntityDomain.Component;
using Application.Interfaces.Cache.LocalizationDomain;
using Application.Interfaces.Cache.MetaDomain;
using Application.Interfaces.Cache.WorldDomain;
using Infrastructure.Cache;
using Infrastructure.Cache.EntityDomain;
using Infrastructure.Cache.EntityDomain.Component;
using Infrastructure.Cache.LocalizationDomain;
using Infrastructure.Cache.MetaDomain;
using Infrastructure.Cache.WorldDomain;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configuration
{
    public static class CacheConfiguration
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static IServiceCollection AddCacheConfiguration(
            this IServiceCollection services)
        {
            // Meta
            services.AddSingleton<IEffectCache, EffectCache>();
            services.AddSingleton<IItemCache, ItemCache>();

            // Entity
            services.AddSingleton<IAICache, AICache>();
            services.AddSingleton<IAppearanceCache, AppearanceCache>();
            services.AddSingleton<ICollisionCache, CollisionCache>();
            services.AddSingleton<ICharacteristicCache, CharacteristicCache>();
            services.AddSingleton<IInteractableCache, InteractableCache>();
            services.AddSingleton<IInventoryCache, InventoryCache>();
            services.AddSingleton<ILifetimeCache, LifetimeCache>();
            services.AddSingleton<IPortalCache, PortalCache>();
            services.AddSingleton<IProjectileCache, ProjectileCache>();
            services.AddSingleton<ITriggeredEffectCache, TriggeredEffectCache>();
            services.AddSingleton<IEntityCache, EntityCache>();

            // World 
            services.AddSingleton<IRoomCache, RoomCache>();
            services.AddSingleton<IRoomConnectionCache, RoomConnectionCache>();

            // Localization
            services.AddSingleton<ILocaleCache, LocaleCache>();

            // Loader
            services.AddScoped<ICacheProvider, CacheProvider>();

            return services;
        }
        #endregion
    }
}