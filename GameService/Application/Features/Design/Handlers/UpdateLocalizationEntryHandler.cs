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

            // Resolve repository
            var localeRepo = relationalUoW.GetRepository<ILocaleRepository>();

            // Validate entry existence
            var entry = await localeRepo.GetByKeyAsync(dto.LocaleCode, dto.Key);
            if (entry == null)
                throw new BadRequest(
                    ApplicationCode.DesignHandlerCode.LocalizationEntryNotFound,
                    $"Localization key '{dto.Key}' was not found under locale '{dto.LocaleCode}'.");

            // Apply peristence - Save changes
            entry.Update(dto.Value, dto.Description);
            await relationalUoW.SaveChangesAsync();
        }
        #endregion
    }
}