using Contract.DTO.Definition.LocalizationDomain;

namespace Application.Interface.Cache.LocalizationDomain
{
    public interface ILocaleCache
    {
        void Load(
            IEnumerable<LocaleDTO> data);
        IReadOnlyCollection<LocaleDTO> GetAll();
    }
}
