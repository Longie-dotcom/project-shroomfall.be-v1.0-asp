using Application.System.Queue;
using Application.System.System;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Configuration
{
    public static class SystemConfiguration
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static IServiceCollection AddSystemConfiguration(
            this IServiceCollection services)
        {
            // QUEUE
            services.AddTransient<CommandBuffer>();

            // SYSTEM
            services.AddSingleton<EntityRequest>();
            services.AddSingleton<EntityResolver>();
            services.AddSingleton<EntityTrigger>();

            return services;
        }
        #endregion
    }
}