using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Domain.Definition.LocalizationDomain;

namespace Application.Services.DesignService
{
    public class LocalizationEntryFactory
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        #endregion

        #region Properties
        #endregion

        public LocalizationEntryFactory(
            IRelationalUoW relationalUoW)
        {
            this.relationalUoW = relationalUoW;
        }

        #region Methods
        public async Task PreSavePlaceholderKeysAsync(LocalizedText keys)
        {
            var localeRepo = relationalUoW.GetRepository<ILocaleRepository>();
            var activeLocales = await localeRepo.GetAllAsync();

            var pendingEntries = new List<LocalizationEntry>();

            foreach (var locale in activeLocales)
            {
                pendingEntries.Add(new LocalizationEntry(Guid.NewGuid(), keys.NameKey, locale.Code, string.Empty));
                pendingEntries.Add(new LocalizationEntry(Guid.NewGuid(), keys.DescriptionKey, locale.Code, string.Empty));
            }

            await localeRepo.SaveLocalizationEntriesAsync(pendingEntries);
        }
        #endregion
    }
}