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
using Application.Services.AttributeService;
using Application.Services.DesignService;
using Application.Services.EntityService;
using Application.Services.IdentityService;
using Application.Services.UsageService;
using Application.Services.WorldService;
using Application.Services.WorldService.Factory;
using Application.Services.WorldService.Factory.Component;
using Application.Services.WorldService.Persistence;
using Application.Systems.Queue;
using Application.Systems.System;
using Contract.DTO.Common;
using Contract.DTO.Definition.EntityDomain.Component;
using Contract.DTO.Definition.MetaDomain;
using Contract.DTO.Definition.WorldDomain;
using Contract.DTO.Feature.Connection.Response;
using Contract.DTO.Feature.Design.Response;
using Contract.DTO.Feature.Identity.Response;
using Contract.DTO.Runtime.WorldDomain;
using Domain.Runtime.WorldDomain.Run;
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
            services.AddScoped<IHandler<FetchEffectDefinitionCommand, PagedResponseDTO<EffectDefinitionDTO>>, FetchEffectDefinitionHandler>();
            services.AddScoped<IHandler<FetchEntityDefinitionCommand, PagedResponseDTO<EntityDefinitionDTO>>, FetchEntityDefinitionHandler>();
            services.AddScoped<IHandler<FetchEntityDefinitionDetailCommand, EntityDefinitionDTO>, FetchEntityDefinitionDetailHandler>();
            services.AddScoped<IHandler<FetchItemDefinitionCommand, PagedResponseDTO<ItemDefinitionDTO>>, FetchItemDefinitionHandler>();
            services.AddScoped<IHandler<FetchRoomDefinitionCommand, PagedResponseDTO<RoomDefinitionDTO>>, FetchRoomDefinitionHandler>();
            services.AddScoped<IHandler<UpdateDefinitionCommand>, UpdateDefinitionHandler>();
            services.AddScoped<IHandler<UpsertEffectDefinitionCommand>, UpsertEffectDefinitionHandler>();
            services.AddScoped<IHandler<UpsertEntityDefinitionCommand>, UpsertEntityDefinitionHandler>();
            services.AddScoped<IHandler<UpsertItemDefinitionCommand>, UpsertItemDefinitionHandler>();
            services.AddScoped<IHandler<UpsertRoomDefinitionCommand>, UpsertRoomDefinitionHandler>();
            services.AddScoped<IHandler<UserRefreshCommand, DefinitionSnapshotDTO?>, UserRefreshHandler>();

            // Game
            services.AddScoped<IHandler<BackHomeCommand, RoomSpatialDTO>, BackHomeHandler>();
            services.AddScoped<IHandler<EnterHubCommand, RoomSpatialDTO>, EnterHubHandler>();
            services.AddScoped<IHandler<MoveCommand>, MoveHandler>();
            services.AddScoped<IHandler<UpdateAppearanceCommand>, UpdateAppearanceHandler>();
            services.AddScoped<IHandler<UseItemCommand>, UseItemHandler>();

            // Identity
            services.AddScoped<IHandler<LoginCommand, TokenDTO>, LoginHandler>();
            services.AddScoped<IHandler<RefreshTokenCommand, TokenDTO>, RefreshTokenHandler>();
            services.AddScoped<IHandler<RegisterCommand, TokenDTO>, RegisterHandler>();
            services.AddScoped<IHandler<SteamAuthCommand, TokenDTO>, SteamAuthHandler>();
            services.AddScoped<IHandler<UpdateProfileCommand>, UpdateProfileHandler>();

            // ─────────────────────────────
            // MAPPERS
            // ─────────────────────────────
            services.AddAutoMapper(cfg => { cfg.AddProfile<DTOMapper>(); });
            services.AddAutoMapper(cfg => { cfg.AddProfile<SnapshotMapper>(); });

            // ─────────────────────────────
            // SERVICES
            // ─────────────────────────────
            // Attribute service
            services.AddSingleton<CharacteristicService>();
            services.AddSingleton<DeathService>();
            services.AddSingleton<EffectService>();
            services.AddSingleton<VitalService>();

            // Design service
            services.AddSingleton<ComponentDiscoveryRegistry>();
            services.AddScoped<DefinitionService>();
            services.AddScoped<DefinitionComponentFactory>();
            services.AddScoped<LocalizationEntryFactory>();

            // Entity service
            services.AddSingleton<AIService>();
            services.AddSingleton<LifetimeService>();
            services.AddSingleton<ProjectileService>();
            services.AddSingleton<TransformService>();
            services.AddSingleton<TriggeredEffectService>();

            // Identity service
            services.AddSingleton<TokenService>();

            // Usage service
            services.AddSingleton<InventoryService>();
            services.AddSingleton<ItemService>();

            // World service
            services.AddSingleton<DefinitionRuntimeFactory>();
            services.AddSingleton<SnapshotRuntimeFactory>();
            services.AddSingleton<EntityInstanceFactory>();
            services.AddSingleton<RoomSpatialFactory>();

            services.AddScoped<EntityPersistence>();
            services.AddScoped<RoomPersistence>();
            services.AddScoped<SnapshotPersistence>();

            services.AddSingleton<BootstrapService>();
            services.AddSingleton<CollisionService>();
            services.AddSingleton<EntitySpawnService>();
            services.AddSingleton<InitializationService>();
            services.AddSingleton<PartyService<CombatRunInstance, CombatRunParticipant>>();
            services.AddSingleton<ResidencyService>();
            services.AddSingleton<RoomMigrationService>();
            services.AddSingleton<WorldContext>();

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