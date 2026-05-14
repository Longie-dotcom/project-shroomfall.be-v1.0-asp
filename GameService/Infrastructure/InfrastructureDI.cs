using Application.Interfaces.Cache;
using Application.Interfaces.Factory;
using Application.Interfaces.Realtime;
using Application.Interfaces.Repository.NonRelational;
using Application.Interfaces.Repository.Relational;
using Application.Interfaces.Security;
using Domain.Abstraction.World;
using Domain.Runtime.WorldDomain;
using Infrastructure.Background;
using Infrastructure.Cache;
using Infrastructure.Factory;
using Infrastructure.Persistence;
using Infrastructure.Realtime;
using Infrastructure.Realtime.Handlers;
using Infrastructure.Repository.NonRelational;
using Infrastructure.Repository.Relational;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
            // SQL SERVER
            // ─────────────────────────────
            var sqlConnection = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");

            services.AddDbContext<RelationalDB>(options =>
                options.UseSqlServer(sqlConnection));

            // ─────────────────────────────
            // MONGODB
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
            // Unit of work
            services.AddScoped<IRelationalUoW, RelationalUoW>();
            services.AddScoped<INonRelationalUoW, NonRelationalUoW>();

            // Relational
            services.AddScoped<IAttributeValueRepository, AttributeValueRepository>();
            services.AddScoped<ICharacteristicRepository, CharacteristicRepository>();
            services.AddScoped<IEffectRepository, EffectRepository>();
            services.AddScoped<IEntityRepository, EntityRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<ILocaleRepository, LocaleRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<ITileRepository, TileRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDefinitionVersionLogRepository, DefinitionVersionLogRepository>();

            // Non-relational
            services.AddScoped<IEntityDocumentRepository, EntityDocumentRepository>();
            services.AddScoped<IRoomDocumentRepository, RoomDocumentRepository>();

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
            services.AddSingleton<IAttributeValueCache, AttributeValueCache>();
            services.AddSingleton<ICharacteristicCache, CharacteristicCache>();
            services.AddSingleton<IEffectCache, EffectCache>();
            services.AddSingleton<IEntityCache, EntityCache>();
            services.AddSingleton<IInventoryCache, InventoryCache>();
            services.AddSingleton<IItemCache, ItemCache>();
            services.AddSingleton<ILocaleCache, LocaleCache>();
            services.AddSingleton<IRoomCache, RoomCache>();
            services.AddSingleton<ITileCache, TileCache>();
            services.AddScoped<ICacheLoader, CacheLoader>();

            // ─────────────────────────────
            // FACTORIES
            // ─────────────────────────────
            services.AddScoped<ICreatureInstanceFactory, CreatureInstanceFactory>();
            services.AddScoped<ICharacteristicInstanceFactory, CharacteristicInstanceFactory>();
            services.AddScoped<IEffectInstanceFactory, EffectInstanceFactory>();
            services.AddScoped<IEntityInstanceFactory, EntityInstanceFactory>();
            services.AddScoped<IInventoryInstanceFactory, InventoryInstanceFactory>();
            services.AddScoped<IItemInstanceFactory, ItemInstanceFactory>();
            services.AddScoped<IPlayerInstanceFactory, PlayerInstanceFactory>();
            services.AddScoped<IRoomSpatialFactory, RoomSpatialFactory>();
            services.AddScoped<IWorldObjectInstanceFactory, WorldObjectInstanceFactory>();

            // ─────────────────────────────
            // BACKGROUND
            // ─────────────────────────────
            services.AddHostedService<WorldLoopService>();

            // ─────────────────────────────
            // REALTIME
            // ─────────────────────────────
            // Core Realtime
            services.AddSignalRCore();
            services.AddSingleton<IRealtimePublisher, RealtimePublisher>();

            // Connection
            services.AddSingleton<IConnectionRegistry, ConnectionRegistry>();
            services.AddSingleton<IConnectionManager, ConnectionManager>();

            // Handlers
            services.AddSingleton<IEventHandler, DefinitionUpdatedHandler>();
            services.AddSingleton<IEventHandler, EntityLifecycleHandler>();
            services.AddSingleton<IEventHandler, EntityMovedHandler>();
            services.AddSingleton<IEventHandler, PlayerGroupedHandler>();

            // Events
            services.AddSingleton<IEventBus, EventBus>();
            services.AddSingleton<IEventDispatcher, EventDispatcher>();

            // ─────────────────────────────
            // SESSION MANAGER
            // ─────────────────────────────
            services.AddSingleton<ISessionManager, SessionManager>();

            // ─────────────────────────────
            // JWT TOKEN
            // ─────────────────────────────
            services.AddSingleton<ITokenGenerator>(sp =>
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

            services.AddScoped<ISteamValidator, SteamValidator>(sp =>
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