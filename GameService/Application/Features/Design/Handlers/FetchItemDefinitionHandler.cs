using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using AutoMapper;
using Contract.DTO.Common;
using Contract.DTO.Domain.Definition;

namespace Application.Features.Design.Handlers
{
    public class FetchItemDefinitionHandler : IHandler<FetchItemDefinitionCommand, PagedResponseDTO<ItemDefinitionDTO>>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly IMapper mapper;
        #endregion

        #region Properties
        #endregion

        public FetchItemDefinitionHandler(
            IRelationalUoW relationalUoW,
            IMapper mapper)
        {
            this.relationalUoW = relationalUoW;
            this.mapper = mapper;
        }

        #region Methods
        public async Task<PagedResponseDTO<ItemDefinitionDTO>> Handle(
            FetchItemDefinitionCommand command)
        {
            var queries = command.Queries;

            // Enforce safe boundaries defaults
            int pageNumber = queries.PageNumber < 1 ? 1 : queries.PageNumber;
            int pageSize = queries.PageSize < 1 ? 10 : queries.PageSize;

            var itemRepo = relationalUoW.GetRepository<IItemDefinitionRepository>();

            var (entities, totalCount) = await itemRepo.GetPagedDefinitionsAsync(
                queries?.SearchTerm,
                queries?.Type,
                queries?.Category,
                pageNumber,
                pageSize
            );

            var dtos = mapper.Map<List<ItemDefinitionDTO>>(entities);

            return new PagedResponseDTO<ItemDefinitionDTO>(
                dtos, 
                totalCount, 
                pageNumber, 
                pageSize);
        }
        #endregion
    }
}