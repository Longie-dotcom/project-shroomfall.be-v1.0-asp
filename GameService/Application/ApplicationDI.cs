using Application.Context;
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
using Application.Mapper;
using Application.Persistence;
using Application.Services.AttributeService;
using Application.Services.DesignService;
using Application.Services.EntityService;
using Application.Services.IdentityService;
using Application.Services.ItemService;
using Application.Services.WorldService;
using Application.Services.WorldService.Factory;
using Application.Services.WorldService.Factory.Component;
using Application.Systems.Queue;
using Application.Systems.System;
using Contract.DTO.Connection;
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
            // CONTEXT
            // ─────────────────────────────
            services.AddSingleton<PlayerContext>();
            services.AddSingleton<WorldContext>();

            // ─────────────────────────────
            // FEATURES
            // ─────────────────────────────
            // Core
            services.AddScoped<IDispatcher, Dispatcher>();

            // Connection
            services.AddScoped<IHandler<CreateSessionCommand>, CreateSessionHandler>();
            services.AddScoped<IHandler<FetchSessionCommand, ExistedSessionDTO>, FetchSessionHandler>();
            services.AddScoped<IHandler<LoadSessionCommand, SaveGameDTO>, LoadSessionHandler>();
            services.AddScoped<IHandler<UnloadSessionCommand>, UnloadSessionHandler>();
            services.AddScoped<IHandler<UserConnectCommand>, UserConnectHandler>();

            // Design
            services.AddScoped<IHandler<FetchLocaleCommand, ExistLocalesDTO>, FetchLocaleHandler>();
            services.AddScoped<IHandler<UpdateDefinitionCommand>, UpdateDefinitionHandler>();
            services.AddScoped<IHandler<UserRefreshCommand, DefinitionSnapshotDTO?>, UserRefreshHandler>();

            // Game
            services.AddScoped<IHandler<EnterHubCommand, RoomSnapshotDTO>, EnterHubHandler>();
            services.AddScoped<IHandler<MoveCommand>, MoveHandler>();
            services.AddScoped<IHandler<TouchEntityCommand, RoomSnapshotDTO>, TouchEntityHandler>();
            services.AddScoped<IHandler<UnequipItemCommand>, UnequipItemHandler>();
            services.AddScoped<IHandler<UpdateAppearanceCommand>, UpdateAppearanceHandler>();
            services.AddScoped<IHandler<UseItemCommand>, UseItemHandler>();

            // Identity
            services.AddScoped<IHandler<LoginCommand, TokenDTO>, LoginHandler>();
            services.AddScoped<IHandler<RefreshTokenCommand, TokenDTO>, RefreshTokenHandler>();
            services.AddScoped<IHandler<RegisterCommand, TokenDTO>, RegisterHandler>();
            services.AddScoped<IHandler<SteamAuthCommand, TokenDTO>, SteamAuthHandler>();
            services.AddScoped<IHandler<UpdatePreferredLocaleCommand>, UpdatePreferredLocaleHandler>();
            services.AddScoped<IHandler<UpdateProfileCommand>, UpdateProfileHandler>();

            // ─────────────────────────────
            // MAPPERS
            // ─────────────────────────────
            services.AddAutoMapper(cfg => { cfg.AddProfile<DTOMapper>(); });
            services.AddAutoMapper(cfg => { cfg.AddProfile<SnapshotMapper>(); });

            // ─────────────────────────────
            // PERSISTENCE
            // ─────────────────────────────
            services.AddSingleton<EntityPersistence>();
            services.AddSingleton<RoomConnectionPersistence>();
            services.AddSingleton<RoomPersistence>();
            services.AddSingleton<SnapshotPersistence>();

            // ─────────────────────────────
            // SERVICES
            // ─────────────────────────────
            // Attribute service
            services.AddSingleton<CombatService>();
            services.AddSingleton<CharacteristicService>();
            services.AddSingleton<EffectService>();

            // Design service
            services.AddSingleton<DefinitionService>();

            // Entity service
            services.AddSingleton<AIService>();
            services.AddSingleton<LifetimeService>();
            services.AddSingleton<MovementService>();
            services.AddSingleton<ProjectileService>();

            // Identity service
            services.AddSingleton<TokenService>();

            // Item service
            services.AddSingleton<InventoryService>();
            services.AddSingleton<ItemService>();
            services.AddSingleton<ItemUsageService>();

            // World service
            services.AddSingleton<DefinitionComponentFactory>();
            services.AddSingleton<RuntimeComponentFactory>();
            services.AddSingleton<SnapshotComponentFactory>();
            services.AddSingleton<EntityInstanceFactory>();
            services.AddSingleton<RoomConnectionInstanceFactory>();
            services.AddSingleton<RoomSpatialFactory>();

            services.AddSingleton<BootstrapService>();
            services.AddSingleton<CollisionService>();
            services.AddSingleton<EntitySpawnService>();
            services.AddSingleton<InitializationService>();
            services.AddSingleton<ResidencyService>();
            services.AddSingleton<RoomMigrationService>();
            services.AddSingleton<TopologyService>();

            // ─────────────────────────────
            // SYSTEMS
            // ─────────────────────────────
            // Queue
            services.AddTransient<CommandBuffer>();

            // System
            services.AddSingleton<EntityRequest>();
            services.AddSingleton<EntityResolver>();
            services.AddSingleton<EntityTrigger>();

            return services;
        }
        #endregion
    }
}