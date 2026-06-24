using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Application.Services.DesignService;
using AutoMapper;
using Contract.DTO.Domain.Definition;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Application.Features.Design.Handlers
{
    public class FetchEntityDefinitionDetailHandler : IHandler<FetchEntityDefinitionDetailCommand, EntityDefinitionDetailDTO>
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
        public async Task<EntityDefinitionDetailDTO> Handle(
            FetchEntityDefinitionDetailCommand command)
        {
            var entityRepo = relationalUoW.GetRepository<IEntityDefinitionRepository>();
            var rootEntity = await entityRepo.GetByIdAsync(command.ID);

            if (rootEntity == null)
                throw new NotFound(
                    ApplicationCode.DesignHandlerCode.EntityDefinitionNotFound,
                    $"Entity variant definition configuration targets containing the ID '{command.ID}' could not be resolved.");

            var detailDto = mapper.Map<EntityDefinitionDetailDTO>(rootEntity);
            var componentList = new List<ComponentDefinitionDTO>();

            foreach (var pipeline in discoveryRegistry.GetPipelines())
            {
                var repositoryInstance = pipeline.GetRepoMethod.Invoke(relationalUoW, null);
                if (repositoryInstance == null) continue;

                var taskResult = (Task)pipeline.GetByEntityIdMethod.Invoke(repositoryInstance, new object[] { command.ID })!;
                await taskResult;

                var domainComponent = taskResult.GetType().GetProperty("Result")?.GetValue(taskResult);

                if (domainComponent != null)
                {
                    var mappedDto = mapper.Map(domainComponent, domainComponent.GetType(), pipeline.DtoType);
                    if (mappedDto is ComponentDefinitionDTO componentDto)
                    {
                        componentList.Add(componentDto);
                    }
                }
            }

            detailDto.Components = componentList;
            return detailDto;
        }
        #endregion
    }
}