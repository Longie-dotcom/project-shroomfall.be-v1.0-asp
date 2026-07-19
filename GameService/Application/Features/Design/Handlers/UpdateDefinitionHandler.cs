using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Cache;
using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Design;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Contract;
using Domain.Definition;

namespace Application.Features.Design.Handlers
{
    public class UpdateDefinitionHandler : IHandler<UpdateDefinitionCommand>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly ICacheProvider cacheLoader;
        private readonly IEventBus eventBus;
        #endregion

        #region Properties
        #endregion

        public UpdateDefinitionHandler(
            IRelationalUoW relationalUoW,
            ICacheProvider cacheLoader,
            IEventBus eventBus)
        {
            this.relationalUoW = relationalUoW;
            this.cacheLoader = cacheLoader;
            this.eventBus = eventBus;
        }

        #region Methods
        public async Task Handle(
            UpdateDefinitionCommand command)
        {
            var dto = command.DTO;

            // Resolve repository
            var definitionVersionLogRepo = relationalUoW.GetRepository<IDefinitionVersionLogRepository>();

            // Generate latest version for this key
            var key = dto.Key ?? Constraint.GLOBAL_DEFINITION_VERSION;
            var latest = await definitionVersionLogRepo.GetLatest(key);
            var nextVersion = latest == null ? 1 : latest.Version + 1;

            // Apply domain - Create new version log
            var log = new DefinitionVersionLog(
                Guid.NewGuid().ToString(),
                key,
                nextVersion,
                dto.Description
            );

            // Apply peristence - Save changes
            await definitionVersionLogRepo.AddAsync(log);
            await relationalUoW.SaveChangesAsync();

            // Reload cache - Note: improve by using key to reload needed cache only
            await cacheLoader.LoadAllAsync();

            // Publish realtime invalidation event
            eventBus.Publish(new DefinitionUpdatedEvent(key, nextVersion));
        }
        #endregion
    }
}