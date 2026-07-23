using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using AutoMapper;
using Contract.DTO.Common;
using Contract.DTO.Definition.MetaDomain;

namespace Application.Features.Design.Handlers
{
    public class FetchEffectDefinitionHandler : IHandler<FetchEffectDefinitionCommand, PagedResponseDTO<EffectDefinitionDTO>>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly IMapper mapper;
        #endregion

        #region Properties
        #endregion

        public FetchEffectDefinitionHandler(
            IRelationalUoW relationalUoW,
            IMapper mapper)
        {
            this.relationalUoW = relationalUoW;
            this.mapper = mapper;
        }

        #region Methods
        public async Task<PagedResponseDTO<EffectDefinitionDTO>> Handle(
            FetchEffectDefinitionCommand command)
        {
            var queries = command.Queries;

            // Enforce safe boundaries defaults
            int pageNumber = queries.PageNumber < 1 ? 1 : queries.PageNumber;
            int pageSize = queries.PageSize < 1 ? 10 : queries.PageSize;

            // Retrieve effect definition and paging
            var effectRepo = relationalUoW.GetRepository<IEffectDefinitionRepository>();
            var (entities, totalCount) = await effectRepo.GetPagedDefinitionsAsync(
                queries?.SearchTerm,
                queries?.Type,
                queries?.AttributeType,
                pageNumber,
                pageSize);

            // Map to result
            var dtos = mapper.Map<List<EffectDefinitionDTO>>(entities);
            return new PagedResponseDTO<EffectDefinitionDTO>(
                dtos,
                totalCount,
                pageNumber,
                pageSize);
        }
        #endregion
    }
}