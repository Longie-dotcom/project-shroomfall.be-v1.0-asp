using Application.Interfaces.Repository.Relational;
using Domain.Definition.LocalizationDomain;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Relational
{
    public class LocaleRepository : SQLGenericRepository<Locale>, ILocaleRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public LocaleRepository(RelationalDB context) : base(context) { }

        #region Methods
        public override async Task<IEnumerable<Locale>> GetAllAsync()
        {
            return await dbSet
                .Include(l => l.LocalizationEntries)
                .ToListAsync();
        }

        /// <summary>
        /// Explicitly inserts a collection of child localization entries into the database.
        /// </summary>
        public async Task SaveLocalizationEntriesAsync(
            IEnumerable<LocalizationEntry> localizationEntries)
        {
            if (localizationEntries == null || !localizationEntries.Any()) return;

            await context.Set<LocalizationEntry>().AddRangeAsync(localizationEntries);
        }

        /// <summary>
        /// Purges all existing localization entries attached to a locale and swaps them with an overwritten dataset.
        /// </summary>
        public async Task ReplaceLocalizationEntriesAsync(
            string localeCode,
            IEnumerable<LocalizationEntry> newEntries)
        {
            var oldEntries = await context.Set<LocalizationEntry>()
                .Where(e => e.LocaleCode == localeCode)
                .ToListAsync();

            if (oldEntries.Any())
            {
                context.Set<LocalizationEntry>().RemoveRange(oldEntries);
            }

            if (newEntries != null && newEntries.Any())
            {
                await context.Set<LocalizationEntry>().AddRangeAsync(newEntries);
            }
        }
        #endregion
    }
}