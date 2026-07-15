using Application.Interfaces.Realtime;
using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Managers;
using Infrastructure.Realtime;
using Infrastructure.Realtime.Events;
using Infrastructure.Realtime.Events.Admin;
using Infrastructure.Realtime.Events.Design;
using Infrastructure.Realtime.Events.Game;
using Infrastructure.Realtime.Managers;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configuration
{
    public static class RealtimeConfiguration
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static IServiceCollection AddRealtimeConfiguration(
            this IServiceCollection services)
        {
            // Core Realtime
            services.AddSignalR();
            services.AddSingleton<IRealtimePublisher, RealtimePublisher>();

            // Managers
            services.AddSingleton<ISessionManager, SessionManager>();
            services.AddSingleton<IConnectionManager, ConnectionManager>();

            // Handlers - Admin
            services.AddSingleton<IEventHandler, RoomResidencyChangedHandler>();
            services.AddSingleton<IEventHandler, UserConnectionChangedHandler>();
            services.AddSingleton<IEventHandler, UserSessionChangedHandler>();

            // Handlers - Design
            services.AddSingleton<IEventHandler, DefinitionUpdatedHandler>();
            
            // Handlers - Game
            services.AddSingleton<IEventHandler, EntityActedHandler>();
            services.AddSingleton<IEventHandler, EntityLifecycleHandler>();
            services.AddSingleton<IEventHandler, EntityVitalChangedHandler>();
            services.AddSingleton<IEventHandler, InventoryClearedHandler>();
            services.AddSingleton<IEventHandler, InventoryItemChangedHandler>();
            services.AddSingleton<IEventHandler, PlayerAppearanceChangedHandler>();
            services.AddSingleton<IEventHandler, PlayerCharacteristicSyncHandler>();
            services.AddSingleton<IEventHandler, RoomSnapshotUpdatedHandler>();

            // Events
            services.AddSingleton<IEventBus, EventBus>();
            services.AddSingleton<IEventDispatcher, EventDispatcher>();
            
            return services;
        }
        #endregion
    }
}