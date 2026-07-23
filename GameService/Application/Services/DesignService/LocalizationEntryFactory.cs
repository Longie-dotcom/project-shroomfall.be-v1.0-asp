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
        public async Task PreSavePlaceholderKeysAsync(
            LocalizedText keys)
        {
            // Resolve repository
            var localeRepo = relationalUoW.GetRepository<ILocaleRepository>();

            var pendingEntries = new List<LocalizationEntry>();

            // Retrieve existed locale to populate entries
            var activeLocales = await localeRepo.GetAllAsync();
            foreach (var locale in activeLocales)
            {
                pendingEntries.Add(new LocalizationEntry(Guid.NewGuid(), keys.NameKey, locale.Code, string.Empty));
                pendingEntries.Add(new LocalizationEntry(Guid.NewGuid(), keys.DescriptionKey, locale.Code, string.Empty));
            }

            // Apply persistence
            await localeRepo.SaveLocalizationEntriesAsync(pendingEntries);
        }
        #endregion
    }
}