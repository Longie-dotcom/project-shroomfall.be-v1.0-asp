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

        public async Task<IEnumerable<Locale>> GetAllAsyncWithoutJoined()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<LocalizationEntry?> GetByKeyAsync(
            string localeCode,
            string key)
        {
            var normalizedLocale = localeCode.Trim().ToLowerInvariant();
            var normalizedKey = key.Trim();

            var query = context.Set<LocalizationEntry>().AsQueryable();

            return await query.FirstOrDefaultAsync(x =>
                x.LocaleCode == normalizedLocale &&
                x.Key == normalizedKey &&
                !x.IsDeleted);
        }

        public async Task<(IEnumerable<LocalizationEntry> Items, int TotalCount)> GetPagedDefinitionsAsync(
            string? searchTerm,
            string localeCode,
            int pageNumber,
            int pageSize)
        {
            // 1. Target the LocalizationEntry DbSet directly since we want to return entries
            var query = context.Set<LocalizationEntry>()
                .AsNoTracking()
                .Where(x => x.LocaleCode == localeCode.Trim().ToLowerInvariant() && !x.IsDeleted);

            // 2. Apply text search against the Key or localized Value
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(x =>
                    x.Key.ToLower().Contains(term) ||
                    x.Value.ToLower().Contains(term));
            }

            // 3. Count total matching elements before slicing the page
            int totalCount = await query.CountAsync();

            // 4. Sort and execute pagination execution pipeline
            var items = await query
                .OrderBy(x => x.Key) // Sorting guarantees predictable page slices
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
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
        #endregion
    }
}