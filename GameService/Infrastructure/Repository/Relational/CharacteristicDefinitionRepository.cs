using Application.Interfaces.Repository.Relational;
using Domain.Definition.EntityDomain.Component;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Relational
{
    public class CharacteristicDefinitionRepository : SQLGenericRepository<CharacteristicDefinition>, ICharacteristicDefinitionRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public CharacteristicDefinitionRepository(RelationalDB context) : base(context) { }

        #region Methods
        public override async Task<IEnumerable<CharacteristicDefinition>> GetAllAsync()
        {
            return await dbSet
                .Include(l => l.AttributeValues)
                .ThenInclude(a => a.AttributeGrowthValues)
                .ToListAsync();
        }
        #endregion
    }
}