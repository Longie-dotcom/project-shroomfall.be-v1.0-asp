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
            Console.WriteLine($"\n[FetchEntityDetail] === Starting Detail Fetch for ID: '{command.ID}' ===");

            var entityRepo = relationalUoW.GetRepository<IEntityDefinitionRepository>();
            var rootEntity = await entityRepo.GetByIdAsync(command.ID);

            if (rootEntity == null)
            {
                Console.WriteLine($"[FetchEntityDetail] [CRITICAL] Core Entity '{command.ID}' not found in DB!");
                throw new NotFound(
                    ApplicationCode.DesignHandlerCode.EntityDefinitionNotFound,
                    $"Entity variant definition configuration targets containing the ID '{command.ID}' could not be resolved.");
            }

            Console.WriteLine($"[FetchEntityDetail] Core entity record found. Mapping base schema...");
            var detailDto = mapper.Map<EntityDefinitionDetailDTO>(rootEntity);
            var componentList = new List<ComponentDefinitionDTO>();

            Console.WriteLine($"[FetchEntityDetail] Processing {discoveryRegistry.GetPipelines().Count} discovered pipelines...");

            foreach (var pipeline in discoveryRegistry.GetPipelines())
            {
                string componentName = pipeline.DtoType.Name.Replace("DTO", "");

                // 1. Resolve Repository Instance
                var repositoryInstance = pipeline.GetRepoMethod.Invoke(relationalUoW, null);
                if (repositoryInstance == null)
                {
                    Console.WriteLine($"  [-] Pipeline '{componentName}': Failed to resolve repository instance from Unit of Work.");
                    continue;
                }

                try
                {
                    // 2. Invoke Async Database query
                    var taskResult = (Task)pipeline.GetByEntityIdMethod.Invoke(repositoryInstance, new object[] { command.ID })!;
                    await taskResult;

                    // 3. Extract reflection task result
                    var domainComponent = taskResult.GetType().GetProperty("Result")?.GetValue(taskResult);

                    if (domainComponent == null)
                    {
                        // This means the DB query ran cleanly but returned NULL (no record exists for this entity ID)
                        Console.WriteLine($"  [.] Pipeline '{componentName}': DB Query returned null (Component not attached to this entity).");
                        continue;
                    }

                    Console.WriteLine($"  [+] Pipeline '{componentName}': Found active DB record! Type: {domainComponent.GetType().Name}");

                    // 4. Map Domain Model to DTO Type
                    var mappedDto = mapper.Map(domainComponent, domainComponent.GetType(), pipeline.DtoType);
                    if (mappedDto == null)
                    {
                        Console.WriteLine($"  [!] Pipeline '{componentName}': AutoMapper returned NULL when transforming domain element to DTO.");
                        continue;
                    }

                    // 5. Type verify and safely append to polymorphic output collection
                    if (mappedDto is ComponentDefinitionDTO componentDto)
                    {
                        componentList.Add(componentDto);
                        Console.WriteLine($"  [SUCCESS] Pipeline '{componentName}': Successfully mapped and appended {mappedDto.GetType().Name} to component array.");
                    }
                    else
                    {
                        Console.WriteLine($"  [!] Pipeline '{componentName}': Mapped object type '{mappedDto.GetType().Name}' does not inherit from ComponentDefinitionDTO!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  [ERROR] Pipeline '{componentName}': Exception occurred during extraction. Message: {ex.InnerException?.Message ?? ex.Message}");
                }
            }

            detailDto.Components = componentList;

            // Final DTO Breakdown Logging Setup
            Console.WriteLine("\n[FetchEntityDetail] === FINAL OUTPUT DTO BREAKDOWN ===");
            Console.WriteLine($"ID:          {detailDto.ID}");
            Console.WriteLine($"Type:        {detailDto.Type}");
            Console.WriteLine($"Total Comp:  {detailDto.Components.Count}");

            try
            {
                var jsonDump = System.Text.Json.JsonSerializer.Serialize(detailDto, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });
                Console.WriteLine("[FetchEntityDetail] Raw Transmitted JSON Payload Structure:\n" + jsonDump);
            }
            catch (Exception jsonEx)
            {
                Console.WriteLine($"[FetchEntityDetail] Could not string serialize output payload: {jsonEx.Message}");
            }

            Console.WriteLine("[FetchEntityDetail] ==================================================\n");

            return detailDto;
        }
        #endregion
    }
}