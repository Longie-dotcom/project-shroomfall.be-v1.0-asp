using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using AutoMapper;
using Contract.DTO.Common;
using Contract.DTO.Domain.Definition;

namespace Application.Features.Design.Handlers
{
    public class FetchEntityDefinitionHandler : IHandler<FetchEntityDefinitionCommand, PagedResponseDTO<EntityDefinitionDTO>>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly IMapper mapper;
        #endregion

        #region Properties
        #endregion

        public FetchEntityDefinitionHandler(
            IRelationalUoW relationalUoW,
            IMapper mapper)
        {
            this.relationalUoW = relationalUoW;
            this.mapper = mapper;
        }

        #region Methods
        public async Task<PagedResponseDTO<EntityDefinitionDTO>> Handle(
            FetchEntityDefinitionCommand command)
        {
            var queries = command.Queries;
            var entityRepo = relationalUoW.GetRepository<IEntityDefinitionRepository>();

            var entities = await entityRepo.GetAllAsync();

            // Apply query filters if filled out
            if (!string.IsNullOrWhiteSpace(queries.SearchTerm))
            {
                entities = entities.Where(e => e.ID.Contains(queries.SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            // Track full filtered database total dataset records count
            var totalCount = entities.Count();

            // Slice data collection using targeted Skip/Take math formulas
            var pagedEntities = entities
                .Skip((queries.PageNumber - 1) * queries.PageSize)
                .Take(queries.PageSize)
                .ToList();

            var mappedItems = mapper.Map<List<EntityDefinitionDTO>>(pagedEntities);

            return new PagedResponseDTO<EntityDefinitionDTO>(
                mappedItems,
                totalCount,
                queries.PageNumber,
                queries.PageSize
            );
        }
        #endregion
    }
}