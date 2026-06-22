using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Cache;
using AutoMapper;
using Contract.DTO.Design;
using Contract.DTO.Domain.Definition;

namespace Application.Features.Design.Handlers
{
    public class FetchLocaleHandler : IHandler<FetchLocaleCommand, ExistLocalesDTO>
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly ICacheProvider cacheProvider;
        #endregion

        #region Properties
        #endregion

        public FetchLocaleHandler(
            IMapper mapper,
            ICacheProvider cacheProvider)
        {
            this.mapper = mapper;
            this.cacheProvider = cacheProvider;
        }

        #region Methods
        public async Task<ExistLocalesDTO> Handle(
            FetchLocaleCommand command)
        {
            // Retrieve all existed locale
            var locales = cacheProvider.Locale.GetAll();

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