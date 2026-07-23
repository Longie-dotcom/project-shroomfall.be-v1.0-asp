using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Application.Services.DesignService;
using AutoMapper;
using Contract.DTO.Abstraction;
using Contract.DTO.Definition.EntityDomain.Component;
using Domain.DomainException;
using ResponseCode;

namespace Application.Features.Design.Handlers
{
    public class FetchEntityDefinitionDetailHandler : IHandler<FetchEntityDefinitionDetailCommand, EntityDefinitionDTO>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly IMapper mapper;
        private readonly ComponentDiscoveryRegistry discoveryRegistry;
        #endregion

        #region Properties
        #endregion

        public FetchEntityDefinitionDetailHandler(
            IRelationalUoW relationalUoW,
            IMapper mapper,
            ComponentDiscoveryRegistry discoveryRegistry)
        {
            this.relationalUoW = relationalUoW;
            this.mapper = mapper;
            this.discoveryRegistry = discoveryRegistry;
        }

        #region Methods
        public async Task<EntityDefinitionDTO> Handle(
            FetchEntityDefinitionDetailCommand command)
        {
            // Retrieve entity definition
            var entityRepo = relationalUoW.GetRepository<IEntityDefinitionRepository>();
            var rootEntity = await entityRepo.GetByIdAsync(command.ID);
            if (rootEntity == null)
                throw new NotFound(
                    ApplicationCode.DesignHandlerCode.EntityDefinitionNotFound,
                    $"Entity variant definition configuration targets containing the ID '{command.ID}' could not be resolved.");

            // Retrieve components of the entity
            var componentList = new List<ComponentDefinitionDTO>();
            foreach (var component in discoveryRegistry.GetComponents())
            {
                // Resolve component repository
                var repositoryInstance = component.GetRepositoryMethod.Invoke(relationalUoW, null);
                if (repositoryInstance == null) 
                    continue;

                // Retrieve component data
                var task = (Task)component.GetByEntityIdMethod.Invoke(repositoryInstance, new object[] { command.ID })!;
                await task;

                // Map result to DTO
                var domainComponent = task.GetType().GetProperty("Result")?.GetValue(task);
                if (domainComponent != null)
                {
                    var mappedDto = mapper.Map(domainComponent, domainComponent.GetType(), component.DtoType);
                    if (mappedDto is ComponentDefinitionDTO componentDto)
                    {
                        componentList.Add(componentDto);
                    }
                }
            }

            // Map to result
            var detailDto = mapper.Map<EntityDefinitionDTO>(rootEntity);
            detailDto.Components = componentList;
            return detailDto;
        }
        #endregion
    }
}