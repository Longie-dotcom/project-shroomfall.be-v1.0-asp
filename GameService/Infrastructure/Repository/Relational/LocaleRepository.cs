using Application.Interfaces.Repository.Relational;
using Domain.Definition.LocalizationDomain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Relational
{
    public class LocaleRepository : SQLGenericRepository<Locale>, ILocaleRepository, IRelationalRepository
    {
        #region Attributes
        private readonly RelationalDB context;
        #endregion

        #region Properties
        #endregion

        public LocaleRepository(
            RelationalDB context) : base(
                context)
        {
            this.context = context;
        }

        #region Methods
        public async Task<IEnumerable<Locale>> GetAllWithLocalizationEntriesAsync()
        {
            return await context.Locales
                .Include(x => x.LocalizationEntries)
                .AsNoTracking()
                .ToListAsync();
        }
        #endregion
    }
}