using Domain.Abstraction.World;
using Domain.Runtime.WorldDomain;
using Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class InfrastructureDI
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // ─────────────────────────────
            // PERSISTENCES
            // ─────────────────────────────
            services.AddPersistenceConfiguration();

            // ─────────────────────────────
            // REPOSITORIES
            // ─────────────────────────────
            services.AddRepositoryConfiguration();

            // ─────────────────────────────
            // RUNTIME WORLD
            // ─────────────────────────────
            services.AddSingleton<World>();
            services.AddSingleton<IWorldQuery>(sp => sp.GetRequiredService<World>());
            services.AddSingleton<IEntityCommand>(sp => sp.GetRequiredService<World>());
            services.AddSingleton<IRoomCommand>(sp => sp.GetRequiredService<World>());

            // ─────────────────────────────
            // CACHES
            // ─────────────────────────────
            services.AddCacheConfiguration();

            // ─────────────────────────────
            // BACKGROUND
            // ─────────────────────────────
            services.AddBackgroundConfiguration();

            // ─────────────────────────────
            // REALTIME
            // ─────────────────────────────
            services.AddRealtimeConfiguration();

            // ─────────────────────────────
            // UTILITY
            // ─────────────────────────────
            services.AddUtilityConfiguration();

            return services;
        }
        #endregion
    }
}