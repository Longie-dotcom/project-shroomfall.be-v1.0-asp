using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using AutoMapper;
using Contract.DTO.Definition.LocalizationDomain;

namespace Application.Features.Design.Handlers
{
    public class FetchLocaleHandler : IHandler<FetchLocaleCommand, List<LocaleDTO>>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly IMapper mapper;
        #endregion

        #region Properties
        #endregion

        public FetchLocaleHandler(
            IRelationalUoW relationalUoW,
            IMapper mapper)
        {
            this.relationalUoW = relationalUoW;
            this.mapper = mapper;
        }

        #region Methods
        public async Task<List<LocaleDTO>> Handle(
            FetchLocaleCommand command)
        {
            var localeRepo = relationalUoW.GetRepository<ILocaleRepository>();
            var entities = await localeRepo.GetAllAsyncWithoutJoined();
            return mapper.Map<List<LocaleDTO>>(entities);
        }
        #endregion
    }
}