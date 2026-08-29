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
            // TELEMETRY
            services.AddHostedService<TelemetryPublishService>();

            // WORLD LOOP
            services.AddHostedService<WorldLoopService>();

            return services;
        }
        #endregion
    }
}