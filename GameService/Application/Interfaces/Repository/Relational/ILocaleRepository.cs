using Application.Interfaces.Repository.Base;
using Domain.Definition.LocalizationDomain;

namespace Application.Interfaces.Repository.Relational
{
    public interface ILocaleRepository : ISQLGenericRepository<Locale>, IRelationalRepository
    {
        Task<IEnumerable<Locale>> GetAllAsyncWithoutJoined();
        Task<(IEnumerable<LocalizationEntry> Items, int TotalCount)> GetPagedDefinitionsAsync(
            string? searchTerm,
            string localeCode,
            int pageNumber,
            int pageSize);
        Task<LocalizationEntry?> GetByKeyAsync(
            string localeCode,
            string key);
        Task SaveLocalizationEntriesAsync(
            IEnumerable<LocalizationEntry> localizationEntries);
    }
}
