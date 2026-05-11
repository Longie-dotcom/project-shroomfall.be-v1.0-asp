using Application.Helper;
using Application.Identity.Commands;
using Microsoft.Extensions.DependencyInjection;
using Application.Features.Abstraction;
using Application.Features.Identity.Commands;
using Application.Features.Identity.Handlers;
using Application.Features;
using Application.Features.Game.Commands;
using Application.Features.Game.Handlers;
using Application.DTO.Identity;
using Application.Systems;
using Application.DTO.Connection;
using Application.Features.Design.Commands;
using Application.Features.Design.Handlers;
using Application.Services.Abstraction.AttributeService;
using Application.Services.Abstraction.ItemService;
using Application.Services.Abstraction.OtherService;
using Application.Services.Abstraction.WorldService;
using Application.Services.AttributeService;
using Application.Services.WorldService;
using Application.Services.OtherService;
using Application.Services.ItemService;
using Application.Systems.System;

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
            // HELPERS
            // ─────────────────────────────
            services.AddAutoMapper(cfg => {
                cfg.AddProfile<Mapper>();
            });

            // ─────────────────────────────
            // SYSTEMS
            // ─────────────────────────────
            services.AddSingleton<EffectTickSystem>();
            services.AddSingleton<EntityLifecycleSystem>();
            services.AddSingleton<MovementSystem>();
            services.AddSingleton<RoomTransitionSystem>();

            // ─────────────────────────────
            // SERVICES
            // ─────────────────────────────
            services.AddSingleton<ICollisionService, CollisionService>();
            services.AddSingleton<ICharacteristicService, CharacteristicService>();
            services.AddSingleton<IEffectService, EffectService>();
            services.AddSingleton<IEquipmentService, EquipmentService>();
            services.AddSingleton<IInventoryService, InventoryService>();
            services.AddSingleton<ISnapshotService, SnapshotService>();
            services.AddSingleton<ISpawnService, SpawnService>();
            services.AddSingleton<ITokenService, TokenService>();

            // ─────────────────────────────
            // FEATURES
            // ─────────────────────────────
            // Core
            services.AddScoped<IDispatcher, Dispatcher>();

            // Identity
            services.AddScoped<IHandler<SteamAuthCommand, TokenDTO>, SteamAuthHandler>();
            services.AddScoped<IHandler<RegisterCommand, TokenDTO>, RegisterHandler>();
            services.AddScoped<IHandler<LoginCommand, TokenDTO>, LoginHandler>();
            services.AddScoped<IHandler<RefreshTokenCommand, TokenDTO>, RefreshTokenHandler>();
            services.AddScoped<IHandler<UpdateProfileCommand>, UpdateProfileHandler>();

            // Connection
            services.AddScoped<IHandler<UserRefreshCommand, DefinitionSnapshotDTO?>, UserRefreshHandler>();

            // Game
            services.AddScoped<IHandler<MoveCommand>, MoveHandler>();

            // Design
            services.AddScoped<IHandler<UpdateDefinitionCommand>, UpdateDefinitionHandler>();

            return services;
        }
        #endregion
    }
}