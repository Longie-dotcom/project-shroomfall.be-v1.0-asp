using Application.Interfaces.Cache.LocalizationDomain;
using Application.Interfaces.Utility;
using Domain.Definition.LocalizationDomain;
using Domain.DomainException;
using ResponseCode;

namespace Infrastructure.Cache.LocalizationDomain
{
    public class LocaleCache : ILocaleCache
    {
        #region Attributes
        private readonly ITelemetryQueue telemetryQueue;
        private Dictionary<string, Locale> map = new();
        private Dictionary<string, Dictionary<string, LocalizationEntry>> entriesByLocale = new();
        private string defaultLocale = string.Empty;
        #endregion

        #region Properties
        #endregion

        public LocaleCache(
            ITelemetryQueue telemetryQueue)
        {
            this.telemetryQueue = telemetryQueue;
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

            if (data.FirstOrDefault(x => x.IsDefault) == null)
                throw new InternalException(
                    InfrastructureCode.LocaleCacheCode.NoDefaultLocale,
                    "No default locale is configured in the localization dataset.");
        }


        public IReadOnlyCollection<Locale> GetAll()
        {
            return map.Values.ToList();
        }

        public string Resolve(string key, string locale)
        {
            if (entriesByLocale.TryGetValue(locale, out var dict) &&
                dict.TryGetValue(key, out var entry))
                return entry.Value;

            if (locale != defaultLocale &&
                entriesByLocale.TryGetValue(defaultLocale, out var fallback) &&
                fallback.TryGetValue(key, out var fallbackEntry))
            {
                telemetryQueue.EnqueueAlert(
                    InfrastructureCode.LocaleCacheCode.FallbackToDefault,
                    $"Localization key '{key}' not found in locale '{locale}'. Falling back to default locale '{defaultLocale}'.",
                    TelemetrySeverity.Warning
                );
                return fallbackEntry.Value;
            }

            telemetryQueue.EnqueueAlert(
                InfrastructureCode.LocaleCacheCode.FallbackToKey,
                $"Localization key '{key}' could not be resolved in locale '{locale}' or default '{defaultLocale}'. Rendering raw key string.",
                TelemetrySeverity.Warning
            );

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