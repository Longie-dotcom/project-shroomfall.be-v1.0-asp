using Infrastructure.Background;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configuration
{
    public static class BackgroundConfiguration
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static IServiceCollection AddBackgroundConfiguration(
            this IServiceCollection services)
        {
            // Telemetry service
            services.AddHostedService<TelemetryPublishService>();

            // World loop service
            services.AddHostedService<WorldLoopService>();

            return services;
        }
        #endregion
    }
}