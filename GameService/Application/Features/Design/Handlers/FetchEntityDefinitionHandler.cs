using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using AutoMapper;
using Contract.DTO.Common;
using Contract.DTO.Definition.EntityDomain.Component;

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

            // Enforce safe boundaries defaults
            int pageNumber = queries.PageNumber < 1 ? 1 : queries.PageNumber;
            int pageSize = queries.PageSize < 1 ? 10 : queries.PageSize;

            var entityRepo = relationalUoW.GetRepository<IEntityDefinitionRepository>();

            var (entities, totalCount) = await entityRepo.GetPagedDefinitionsAsync(
                queries.SearchTerm,
                pageNumber,
                pageSize
            );

            var mappedItems = mapper.Map<List<EntityDefinitionDTO>>(entities);

            return new PagedResponseDTO<EntityDefinitionDTO>(
                mappedItems,
                totalCount,
                pageNumber,
                pageSize
            );
        }
        #endregion
    }
}