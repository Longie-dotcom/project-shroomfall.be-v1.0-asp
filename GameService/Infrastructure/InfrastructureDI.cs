using Infrastructure.Persistence;
using Infrastructure.Repository.Implementation;
using Infrastructure.Repository.Interface;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using MongoDB.Driver;

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
            // SQL SERVER (ENV)
            // ─────────────────────────────
            var sqlConnection = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");

            services.AddDbContext<RelationalDB>(options =>
                options.UseSqlServer(
                    sqlConnection,
                    x => x.MigrationsAssembly(typeof(RelationalDB).Assembly.GetName().Name)
                ));

            // ─────────────────────────────
            // MONGODB (ENV)
            // ─────────────────────────────
            var mongoConnection = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
            var mongoDbName = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME");

            services.AddSingleton<IMongoClient>(_ =>
                new MongoClient(mongoConnection));

            services.AddScoped<NonRelationalDB>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                var database = client.GetDatabase(mongoDbName);

                return new NonRelationalDB(database);
            });

            // ─────────────────────────────
            // REPOSITORIES
            // ─────────────────────────────
            services.AddScoped<IRelationalUoW, RelationalUoW>();
            services.AddScoped<INonRelationalUoW, NonRelationalUoW>();

            services.AddScoped<IUserRepository, UserRepository>();

            // ─────────────────────────────
            // JWT TOKEN
            // ─────────────────────────────
            services.AddSingleton(sp =>
            {
                var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")!;
                var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")!;
                var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")!;

                return new TokenGenerator(jwtKey, issuer, audience);
            });

            // ─────────────────────────────
            // STEAM VALIDATOR
            // ─────────────────────────────
            services.AddHttpClient<SteamValidator>();

            services.AddScoped<SteamValidator>(sp =>
            {
                var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient();

                var apiKey = Environment.GetEnvironmentVariable("STEAM_API_KEY")!;
                var appId = Environment.GetEnvironmentVariable("STEAM_APP_ID")!;

                return new SteamValidator(httpClient, apiKey, appId);
            });

            return services;
        }
        #endregion
    }
}