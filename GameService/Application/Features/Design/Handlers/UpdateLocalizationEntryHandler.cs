using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Domain.DomainException;
using ResponseCode;

namespace Application.Features.Design.Handlers
{
    public class UpdateLocalizationEntryHandler : IHandler<UpdateLocalizationEntryCommand>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        #endregion

        #region Properties
        #endregion

        public UpdateLocalizationEntryHandler(
            IRelationalUoW relationalUoW)
        {
            this.relationalUoW = relationalUoW;
        }

        #region Methods
        public async Task Handle(
            UpdateLocalizationEntryCommand command)
        {
            var dto = command.DTO;

            // Validate entry existence
            var localeRepo = relationalUoW.GetRepository<ILocaleRepository>();
            var entry = await localeRepo.GetByKeyAsync(dto.LocaleCode, dto.Key);
            if (entry == null)
                throw new BadRequest(
                    ApplicationCode.DesignHandlerCode.LocalizationEntryNotFound,
                    $"Localization key '{dto.Key}' was not found under locale '{dto.LocaleCode}'.");

            // Apply domain
            entry.Update(dto.Value, dto.Description);
            
            // Apply persistence
            await relationalUoW.SaveChangesAsync();
        }
        #endregion
    }
}