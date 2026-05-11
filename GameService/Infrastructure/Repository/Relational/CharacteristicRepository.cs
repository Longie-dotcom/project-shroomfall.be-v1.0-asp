using Application.Interfaces.Repository.Relational;
using Domain.Definition.AttributeDomain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Relational
{
    public class CharacteristicRepository : SQLGenericRepository<Characteristic>, ICharacteristicRepository, IRelationalRepository
    {
        #region Attributes
        private readonly RelationalDB context;
        #endregion

        #region Properties
        #endregion

        public CharacteristicRepository(
            RelationalDB context) : base(
                context)
        {
            this.context = context;
        }

        #region Methods
        public async Task<IEnumerable<Characteristic>> GetAllWithAttributeValuesAsync()
        {
            return await context.Characteristics
                .Include(x => x.AttributeValues)
                .AsNoTracking()
                .ToListAsync();
        }
        #endregion
    }
}