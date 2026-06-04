using Application.Interfaces.Cache;
using Domain.Definition.LocalizationDomain;

namespace Infrastructure.Cache
{
    public class LocaleCache : ILocaleCache
    {
        #region Attributes
        private Dictionary<string, Locale> map = new();
        private Dictionary<string, Dictionary<string, LocalizationEntry>> entriesByLocale = new();
        private string defaultLocale = string.Empty;
        #endregion

        #region Properties
        #endregion

        public LocaleCache()
        {

        }

        #region Methods
        public void Load(
            IEnumerable<Locale> data)
        {
            map = data.ToDictionary(x => x.Code);

            entriesByLocale.Clear();

            foreach (var locale in data)
            {
                entriesByLocale[locale.Code] = locale.LocalizationEntries
                    .Where(x => !x.IsDeleted)
                    .ToDictionary(x => x.Key);
            }

            defaultLocale = data
                .FirstOrDefault(x => x.IsDefault)?.Code
                ?? throw new Exception("No default locale configured.");
        }


        public IReadOnlyCollection<Locale> GetAll()
        {
            return map.Values.ToList();
        }

        public string Resolve(
            string key,
            string locale)
        {
            // Requested locale
            if (entriesByLocale.TryGetValue(locale, out var dict) &&
                dict.TryGetValue(key, out var entry))
                return entry.Value;

            // Default locale fallback
            if (locale != defaultLocale &&
                entriesByLocale.TryGetValue(defaultLocale, out var fallback) &&
                fallback.TryGetValue(key, out var fallbackEntry))
                return fallbackEntry.Value;

            // Key fallback
            return key;
        }

        public bool Exists(
            string locale)
        {
            return map.ContainsKey(locale);
        }
        #endregion
    }
}