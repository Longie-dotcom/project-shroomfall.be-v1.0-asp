using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using AutoMapper;
using Contract.DTO.Common;
using Contract.DTO.Definition.WorldDomain;

namespace Application.Features.Design.Handlers
{
    public class FetchRoomDefinitionHandler : IHandler<FetchRoomDefinitionCommand, PagedResponseDTO<RoomDefinitionDTO>>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly IMapper mapper;
        #endregion

        #region Properties
        #endregion

        public FetchRoomDefinitionHandler(
            IRelationalUoW relationalUoW,
            IMapper mapper)
        {
            this.relationalUoW = relationalUoW;
            this.mapper = mapper;
        }

        #region Methods
        public async Task<PagedResponseDTO<RoomDefinitionDTO>> Handle(
            FetchRoomDefinitionCommand command)
        {
            var queries = command.Queries;

            // Enforce safe boundaries defaults
            int pageNumber = queries.PageNumber < 1 ? 1 : queries.PageNumber;
            int pageSize = queries.PageSize < 1 ? 10 : queries.PageSize;

            var roomRepo = relationalUoW.GetRepository<IRoomDefinitionRepository>();

            var (entities, totalCount) = await roomRepo.GetPagedDefinitionsAsync(
                queries?.SearchTerm,
                queries?.Type,
                pageNumber,
                pageSize
            );

            var dtos = mapper.Map<List<RoomDefinitionDTO>>(entities);

            return new PagedResponseDTO<RoomDefinitionDTO>(
                dtos,
                totalCount,
                pageNumber,
                pageSize);
        }
        #endregion
    }
}