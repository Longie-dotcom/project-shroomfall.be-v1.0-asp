using Application.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ApplicationDI
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            // ─────────────────────────────
            // FEATURE
            // ─────────────────────────────
            services.AddFeatureConfiguration();

            // ─────────────────────────────
            // MAPPER
            // ─────────────────────────────
            services.AddMapperConfiguration();

            // ─────────────────────────────
            // SERVICE
            // ─────────────────────────────
            services.AddServiceConfiguration();

            // ─────────────────────────────
            // SYSTEM
            // ─────────────────────────────
            services.AddSystemConfiguration();

            return services;
        }
        #endregion
    }
}