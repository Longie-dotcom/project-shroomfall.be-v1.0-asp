using Domain.Definition.LocalizationDomain;

namespace Application.Interfaces.Cache
{
    public interface ILocaleCache
    {
        void Load(
            IEnumerable<Locale> data);
        IReadOnlyCollection<Locale> GetAll();
        string Resolve(
            string key, 
            string locale);
        bool Exists(
            string locale);
    }
}
