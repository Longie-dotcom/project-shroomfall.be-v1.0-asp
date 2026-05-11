using Domain.Definition.LocalizationDomain;

namespace Application.Interfaces.Repository.Relational
{
    public interface ILocaleRepository : ISQLGenericRepository<Locale>, IRelationalRepository
    {
        Task<IEnumerable<Locale>> GetAllWithLocalizationEntriesAsync();
    }
}
