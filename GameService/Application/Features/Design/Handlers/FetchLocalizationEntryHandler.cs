using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using AutoMapper;
using Contract.DTO.Common;
using Contract.DTO.Definition.LocalizationDomain;

namespace Application.Features.Design.Handlers
{
    public class FetchLocalizationEntryHandler : IHandler<FetchLocalizationEntryCommand, PagedResponseDTO<LocalizationEntryDTO>>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly IMapper mapper;
        #endregion

        #region Properties
        #endregion

        public FetchLocalizationEntryHandler(
            IRelationalUoW relationalUoW,
            IMapper mapper)
        {
            this.relationalUoW = relationalUoW;
            this.mapper = mapper;
        }

        #region Methods
        public async Task<PagedResponseDTO<LocalizationEntryDTO>> Handle(
            FetchLocalizationEntryCommand command)
        {
            var queries = command.Queries;

            // Enforce safe boundaries defaults
            int pageNumber = queries.PageNumber < 1 ? 1 : queries.PageNumber;
            int pageSize = queries.PageSize < 1 ? 10 : queries.PageSize;

            var localizationRepo = relationalUoW.GetRepository<ILocaleRepository>();

            var (entities, totalCount) = await localizationRepo.GetPagedDefinitionsAsync(
                queries.SearchTerm,
                queries.LocaleCode,
                pageNumber,
                pageSize
            );

            var dtos = mapper.Map<List<LocalizationEntryDTO>>(entities);

            return new PagedResponseDTO<LocalizationEntryDTO>(
                dtos,
                totalCount,
                pageNumber,
                pageSize);
        }
        #endregion
    }
}