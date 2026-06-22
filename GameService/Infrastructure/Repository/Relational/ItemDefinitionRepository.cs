using Application.Interfaces.Repository.Relational;
using Domain.Definition.MetaDomain;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;

namespace Infrastructure.Repository.Relational
{
    public class ItemDefinitionRepository : SQLGenericRepository<ItemDefinition>, IItemDefinitionRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public ItemDefinitionRepository(RelationalDB context) : base(context) { }

        #region Methods
        #endregion
    }
}