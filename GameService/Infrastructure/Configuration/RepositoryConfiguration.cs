using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.NonRelational;
using Application.Interfaces.Repository.Relational;
using Infrastructure.Repository.Base;
using Infrastructure.Repository.NonRelational;
using Infrastructure.Repository.Relational;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configuration
{
    public static class RepositoryConfiguration
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static IServiceCollection AddRepositoryConfiguration(
            this IServiceCollection services)
        {
            // Unit of Work
            services.AddScoped<INonRelationalUoW, NonRelationalUoW>();
            services.AddScoped<IRelationalUoW, RelationalUoW>();

            // Snapshot repositories
            services.AddScoped<IEntitySnapshotRepository, EntitySnapshotRepository>();
            services.AddScoped<IRoomSnapshotRepository, RoomSnapshotRepository>();

            // Entity domain
            services.AddScoped<IAIDefinitionRepository, AIDefinitionRepository>();
            services.AddScoped<IAppearanceDefinitionRepository, AppearanceDefinitionRepository>();
            services.AddScoped<ICollisionDefinitionRepository, CollisionDefinitionRepository>();
            services.AddScoped<ICharacteristicDefinitionRepository, CharacteristicDefinitionRepository>();
            services.AddScoped<IInteractableDefinitionRepository, InteractableDefinitionRepository>();
            services.AddScoped<IInventoryDefinitionRepository, InventoryDefinitionRepository>();
            services.AddScoped<ILifetimeDefinitionRepository, LifetimeDefinitionRepository>();
            services.AddScoped<IPortalDefinitionRepository, PortalDefinitionRepository>();
            services.AddScoped<IProjectileDefinitionRepository, ProjectileDefinitionRepository>();
            services.AddScoped<ITriggeredEffectDefinitionRepository, TriggeredEffectDefinitionRepository>();
            services.AddScoped<IEntityDefinitionRepository, EntityDefinitionRepository>();

            // Identity domain
            services.AddScoped<IUserRepository, UserRepository>();

            // Localization domain
            services.AddScoped<ILocaleRepository, LocaleRepository>();

            // Meta domain
            services.AddScoped<IEffectDefinitionRepository, EffectDefinitionRepository>();
            services.AddScoped<IItemDefinitionRepository, ItemDefinitionRepository>();

            // World domain
            services.AddScoped<ICombatRunDefinitionRepository, CombatRunDefinitionRepository>();
            services.AddScoped<IRoomDefinitionRepository, RoomDefinitionRepository>();

            // Global
            services.AddScoped<IDefinitionVersionLogRepository, DefinitionVersionLogRepository>();

            return services;
        }
        #endregion
    }
}