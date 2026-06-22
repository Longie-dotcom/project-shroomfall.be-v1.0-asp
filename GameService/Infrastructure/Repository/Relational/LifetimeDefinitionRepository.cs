using Application.Interfaces.Repository.Relational;
using Domain.Definition.EntityDomain.Component;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;

namespace Infrastructure.Repository.Relational
{
    public class LifetimeDefinitionRepository : SQLGenericRepository<LifetimeDefinition>, ILifetimeDefinitionRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public LifetimeDefinitionRepository(RelationalDB context) : base(context) { }

        #region Methods
        #endregion
    }
}