using Application.Interfaces.Utility;
using Domain.DomainException;
using Infrastructure.Utility;
using Microsoft.Extensions.DependencyInjection;
using ResponseCode;

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
            // Telemetry queue
            services.AddSingleton<ITelemetryQueue, TelemetryQueue>();

            // JWT token
            services.AddSingleton<ITokenGenerator, TokenGenerator>(sp =>
            {
                var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
                if (string.IsNullOrWhiteSpace(jwtKey))
                    throw new InternalException(
                        InfrastructureCode.UtilityConfigurationCode.JwtKeyMissing,
                        "Critical infrastructure configuration missing. Environment variable 'JWT_KEY' was not found.");

                var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
                if (string.IsNullOrWhiteSpace(issuer))
                    throw new InternalException(
                        InfrastructureCode.UtilityConfigurationCode.JwtIssuerMissing,
                        "Critical infrastructure configuration missing. Environment variable 'JWT_ISSUER' was not found.");

                var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
                if (string.IsNullOrWhiteSpace(audience))
                    throw new InternalException(
                        InfrastructureCode.UtilityConfigurationCode.JwtAudienceMissing,
                        "Critical infrastructure configuration missing. Environment variable 'JWT_AUDIENCE' was not found.");

                return new TokenGenerator(jwtKey, issuer, audience);
            });

            // Steam validation
            services.AddHttpClient<SteamValidator>();

            services.AddScoped<ISteamValidator, SteamValidator>(sp =>
            {
                var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient();

                var apiKey = Environment.GetEnvironmentVariable("STEAM_API_KEY");
                if (string.IsNullOrWhiteSpace(apiKey))
                    throw new InternalException(
                        InfrastructureCode.UtilityConfigurationCode.SteamApiKeyMissing,
                        "Critical infrastructure configuration missing. Environment variable 'STEAM_API_KEY' was not found.");

                var appId = Environment.GetEnvironmentVariable("STEAM_APP_ID");
                if (string.IsNullOrWhiteSpace(appId))
                    throw new InternalException(
                        InfrastructureCode.UtilityConfigurationCode.SteamAppIdMissing,
                        "Critical infrastructure configuration missing. Environment variable 'STEAM_APP_ID' was not found.");

                return new SteamValidator(httpClient, apiKey, appId);
            });

            return services;
        }
        #endregion
    }
}