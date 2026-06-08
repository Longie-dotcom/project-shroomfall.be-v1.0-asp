using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Cache;
using AutoMapper;
using Contract.DTO.Definition;
using Contract.DTO.Design;

namespace Application.Features.Design.Handlers
{
    public class FetchLocaleHandler : IHandler<FetchLocaleCommand, ExistLocalesDTO>
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly ILocaleCache localeCache;
        #endregion

        #region Properties
        #endregion

        public FetchLocaleHandler(
            IMapper mapper,
            ILocaleCache localeCache)
        {
            this.mapper = mapper;
            this.localeCache = localeCache;
        }

        #region Methods
        public async Task<ExistLocalesDTO> Handle(
            FetchLocaleCommand command)
        {
            // Retrieve all existed locale
            var locales = localeCache.GetAll();

            // Mapping and return
            var mapped = mapper.Map<List<LocaleDTO>>(locales);

            return new ExistLocalesDTO()
            {
                Locales = mapped,
            };
        }
        #endregion
    }
}