using Application.Interface.Utility;
using Infrastructure.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configuration
{
    public static class UtilityConfiguration
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static IServiceCollection AddUtilityConfiguration(
            this IServiceCollection services)
        {
            // TELEMETRY QUEUE
            services.AddSingleton<ITelemetryQueue, TelemetryQueue>();

            return services;
        }
        #endregion
    }
}