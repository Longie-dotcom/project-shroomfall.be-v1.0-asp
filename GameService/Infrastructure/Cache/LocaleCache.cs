using Application.Interfaces.Cache;
using Domain.Definition.LocalizationDomain;
using Domain.Shared;

namespace Infrastructure.Cache
{
    public class LocaleCache : ILocaleCache
    {
        #region Attributes
        private Dictionary<string, Locale> map = new();
        private Dictionary<string, Dictionary<string, LocalizationEntry>> entriesByLocale = new();
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
        }

        public IReadOnlyCollection<Locale> GetAll()
        {
            return map.Values.ToList();
        }

        public string Resolve(
            string key, 
            string locale)
        {
            // Resolved result
            if (entriesByLocale.TryGetValue(locale, out var dict) &&
                dict.TryGetValue(key, out var entry))
                return entry.Value;

            // Fallback result
            if (locale != Constraint.DEFAULT_LOCALIZATION &&
                entriesByLocale.TryGetValue(Constraint.DEFAULT_LOCALIZATION, out var fallback) &&
                fallback.TryGetValue(key, out var fallbackEntry))
                return fallbackEntry.Value;

            // Fallback key
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