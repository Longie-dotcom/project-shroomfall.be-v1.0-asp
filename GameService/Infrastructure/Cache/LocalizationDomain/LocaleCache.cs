using Application.Interface.Cache.LocalizationDomain;
using Contract.DTO.Definition.LocalizationDomain;
using Domain.DomainException;
using ResponseCode;

namespace Infrastructure.Cache.LocalizationDomain
{
    public class LocaleCache : ILocaleCache
    {
        #region Attributes
        private Dictionary<string, LocaleDTO> map = new();
        private Dictionary<string, Dictionary<string, LocalizationEntryDTO>> entriesByLocale = new();
        #endregion

        #region Properties
        #endregion

        public LocaleCache() { }

        #region Methods
        public void Load(
            IEnumerable<LocaleDTO> data)
        {
            map = data.ToDictionary(x => x.Code);

            entriesByLocale.Clear();

            foreach (var locale in data)
            {
                entriesByLocale[locale.Code] = locale.LocalizationEntries.ToDictionary(x => x.Key);
            }

            if (data.FirstOrDefault(x => x.IsDefault) == null)
                throw new InternalException(
                    InfrastructureCode.LocaleCacheCode.NoDefaultLocale,
                    "No default locale is configured in the localization dataset.");
        }


        public IReadOnlyCollection<LocaleDTO> GetAll()
        {
            return map.Values.ToList();
        }
        #endregion
    }
}