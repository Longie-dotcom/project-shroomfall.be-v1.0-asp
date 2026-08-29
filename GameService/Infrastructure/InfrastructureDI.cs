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
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services)
        {
            // ─────────────────────────────
            // BACKGROUND
            // ─────────────────────────────
            services.AddBackgroundConfiguration();

            // ─────────────────────────────
            // CACHE
            // ─────────────────────────────
            services.AddCacheConfiguration();

            // ─────────────────────────────
            // GRPC
            // ─────────────────────────────
            services.AddGrpcConfiguration();

            // ─────────────────────────────
            // MESSAGING
            // ─────────────────────────────
            services.AddMessagingConfiguration();

            // ─────────────────────────────
            // PERSISTENCE
            // ─────────────────────────────
            services.AddPersistenceConfiguration();

            // ─────────────────────────────
            // REALTIME
            // ─────────────────────────────
            services.AddRealtimeConfiguration();

            // ─────────────────────────────
            // REPOSITORY
            // ─────────────────────────────
            services.AddRepositoryConfiguration();

            // ─────────────────────────────
            // UTILITY
            // ─────────────────────────────
            services.AddUtilityConfiguration();

            // ─────────────────────────────
            // RUNTIME WORLD
            // ─────────────────────────────
            services.AddSingleton<World>();
            services.AddSingleton<IWorldQuery>(sp => sp.GetRequiredService<World>());
            services.AddSingleton<IEntityCommand>(sp => sp.GetRequiredService<World>());
            services.AddSingleton<IRoomCommand>(sp => sp.GetRequiredService<World>());

            return services;
        }
        #endregion
    }
}