using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using AutoMapper;
using Contract.DTO.Common;
using Contract.DTO.Definition.WorldDomain;

namespace Application.Features.Design.Handlers
{
    public class FetchCombatRunDefinitionHandler : IHandler<FetchCombatRunDefinitionCommand, PagedResponseDTO<CombatRunDefinitionDTO>>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly IMapper mapper;
        #endregion

        #region Properties
        #endregion

        public FetchCombatRunDefinitionHandler(
            IRelationalUoW relationalUoW,
            IMapper mapper)
        {
            this.relationalUoW = relationalUoW;
            this.mapper = mapper;
        }

        #region Methods
        public async Task<PagedResponseDTO<CombatRunDefinitionDTO>> Handle(
            FetchCombatRunDefinitionCommand command)
        {
            var queries = command.Queries;

            // Enforce safe boundaries defaults
            int pageNumber = queries.PageNumber < 1 ? 1 : queries.PageNumber;
            int pageSize = queries.PageSize < 1 ? 10 : queries.PageSize;

            // Retrieve effect definition and paging
            var combatRunRepo = relationalUoW.GetRepository<ICombatRunDefinitionRepository>();
            var (entities, totalCount) = await combatRunRepo.GetPagedDefinitionsAsync(
                queries?.SearchTerm,
                pageNumber,
                pageSize);

            // Map to result
            var dtos = mapper.Map<List<CombatRunDefinitionDTO>>(entities);
            return new PagedResponseDTO<CombatRunDefinitionDTO>(
                dtos,
                totalCount,
                pageNumber,
                pageSize);
        }
        #endregion
    }
}