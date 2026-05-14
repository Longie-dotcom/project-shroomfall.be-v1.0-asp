using Application.Events.Event;
using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Cache;
using Application.Interfaces.Realtime;
using Application.Interfaces.Repository.Relational;
using Domain.DomainException;
using Domain.Other.VersionDomain;
using Domain.Shared;

namespace Application.Features.Design.Handlers
{
    public class UpdateDefinitionHandler : IHandler<UpdateDefinitionCommand>
    {
        #region Attributes
        private readonly IRelationalUoW relational;
        private readonly ICacheLoader cacheLoader;
        private readonly IEventBus eventBus;
        #endregion

        #region Properties
        #endregion

        public UpdateDefinitionHandler(
            IRelationalUoW relational,
            ICacheLoader cacheLoader,
            IEventBus eventBus)
        {
            this.relational = relational;
            this.cacheLoader = cacheLoader;
            this.eventBus = eventBus;
        }

        #region Methods
        public async Task Handle(UpdateDefinitionCommand command)
        {
            var dto = command.DTO;

            // Resolve repository
            var definitionVersionLogRepo =
                relational.GetRepository<IDefinitionVersionLogRepository>();

            // Validate latest version for this key
            var latest = await definitionVersionLogRepo.GetLatest(dto.Key ?? Constraint.GLOBAL_DEFINITION_VERSION);
            if (latest != null && dto.Version <= latest.Version)
                throw new BadRequest(
                    ResponseCode.UpdateDefinition_InvalidVersion,
                    $"Update definition must has newer version than old version");

            // Apply domain - Create new version log
            var log = new DefinitionVersionLog(
                Guid.NewGuid().ToString(),
                dto.Key ?? Constraint.GLOBAL_DEFINITION_VERSION,
                dto.Version,
                dto.Description
            );

            // Apply peristence - Save changes
            await definitionVersionLogRepo.AddAsync(log);

            // Reload cache - Note: improve by using key to reload needed cache only
            await cacheLoader.LoadAllAsync();

            // Publish realtime invalidation event
            eventBus.Publish(new DefinitionUpdatedEvent(
                dto.Key ?? Constraint.GLOBAL_DEFINITION_VERSION,
                dto.Version)
            );
        }
        #endregion
    }
}