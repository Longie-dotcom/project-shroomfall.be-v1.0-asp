using Application.Bootstrapper;
using Application.Context;
using Application.Coordinator;
using Application.Features;
using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
using Application.Features.Connection.Handlers;
using Application.Features.Design.Commands;
using Application.Features.Design.Handlers;
using Application.Features.Game.Commands;
using Application.Features.Game.Handlers;
using Application.Features.Identity.Commands;
using Application.Features.Identity.Handlers;
using Application.Helper;
using Application.Persistence;
using Application.Services.AttributeService;
using Application.Services.DesignService;
using Application.Services.IdentityService;
using Application.Services.ItemService;
using Application.Services.WorldService;
using Application.Systems.Request;
using Application.Systems.Resolver;
using Application.Systems.Tick;
using Application.Systems.Trigger;
using Contract.DTO.Connection;
using Contract.DTO.Definition;
using Contract.DTO.Design;
using Contract.DTO.Identity;
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
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // ─────────────────────────────
            // BOOTSTRAPPER
            // ─────────────────────────────
            services.AddScoped<TopologyBootstrap>();

            // ─────────────────────────────
            // CONTEXT
            // ─────────────────────────────
            services.AddSingleton<PlayerContext>();
            services.AddSingleton<WorldContext>();

            // ─────────────────────────────
            // COORDINATOR
            // ─────────────────────────────
            services.AddSingleton<PlayerCoordinator>();
            services.AddSingleton<SpawnService>();

            // ─────────────────────────────
            // FEATURES
            // ─────────────────────────────
            // Core
            services.AddScoped<IDispatcher, Dispatcher>();

            // Connection
            services.AddScoped<IHandler<CreateSessionCommand, ExistedSessionEntryDTO>, CreateSessionHandler>();
            services.AddScoped<IHandler<FetchSessionCommand, ExistedSessionDTO>, FetchSessionHandler>();
            services.AddScoped<IHandler<LoadSessionCommand, SaveGameDTO>, LoadSessionHandler>();
            services.AddScoped<IHandler<UnloadSessionCommand>, UnloadSessionHandler>();
            services.AddScoped<IHandler<UserConnectCommand>, UserConnectHandler>();

            // Identity
            services.AddScoped<IHandler<LoginCommand, TokenDTO>, LoginHandler>();
            services.AddScoped<IHandler<RefreshTokenCommand, TokenDTO>, RefreshTokenHandler>();
            services.AddScoped<IHandler<RegisterCommand, TokenDTO>, RegisterHandler>();
            services.AddScoped<IHandler<SteamAuthCommand, TokenDTO>, SteamAuthHandler>();
            services.AddScoped<IHandler<UpdatePreferredLocaleCommand>, UpdatePreferredLocaleHandler>();
            services.AddScoped<IHandler<UpdateProfileCommand>, UpdateProfileHandler>();

            // Game
            services.AddScoped<IHandler<MoveCommand>, MoveHandler>();
            services.AddScoped<IHandler<TouchEntityCommand, RoomSnapshotDTO>, TouchEntityHandler>();
            services.AddScoped<IHandler<UpdateAppearanceCommand>, UpdateAppearanceHandler>();

            // Design
            services.AddScoped<IHandler<FetchLocaleCommand, IEnumerable<LocaleDTO>>, FetchLocaleHandler>();
            services.AddScoped<IHandler<UpdateDefinitionCommand>, UpdateDefinitionHandler>();
            services.AddScoped<IHandler<UserRefreshCommand, DefinitionSnapshotDTO?>, UserRefreshHandler>();

            // ─────────────────────────────
            // HELPERS
            // ─────────────────────────────
            services.AddAutoMapper(cfg => {
                cfg.AddProfile<Mapper>();
            });

            // ─────────────────────────────
            // PERSISTENCE
            // ─────────────────────────────
            services.AddSingleton<EntityPersistence>();
            services.AddSingleton<RoomPersistence>();
            services.AddSingleton<SnapshotPersistence>();

            // ─────────────────────────────
            // SERVICES
            // ─────────────────────────────
            // Attribute service
            services.AddSingleton<CharacteristicService>();
            services.AddSingleton<EffectService>();

            // Design service
            services.AddSingleton<BuilderService>();

            // Identity service
            services.AddSingleton<TokenService>();

            // Item service
            services.AddSingleton<ConsumableService>();
            services.AddSingleton<EquipmentService>();
            services.AddSingleton<InventoryService>();
            services.AddSingleton<ItemService>();
            services.AddSingleton<PlacementService>();

            // World service
            services.AddSingleton<CollisionService>();
            services.AddSingleton<InitializationService>();
            services.AddSingleton<SpawnService>();
            services.AddSingleton<TopologyService>();

            // ─────────────────────────────
            // SYSTEMS
            // ─────────────────────────────
            // Request
            services.AddSingleton<MovementRequest>();

            // Resolver
            services.AddSingleton<CollisionResolver>();

            // Tick
            services.AddSingleton<EffectTick>();
            services.AddSingleton<ResidencyTick>();

            // Trigger
            services.AddSingleton<MovementTrigger>();

            return services;
        }
        #endregion
    }
}