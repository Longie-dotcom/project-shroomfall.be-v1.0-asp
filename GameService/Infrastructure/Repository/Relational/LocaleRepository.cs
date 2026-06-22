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
        #endregion
    }
}