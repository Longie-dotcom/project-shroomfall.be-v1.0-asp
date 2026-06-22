using Application.Interfaces.Repository.Relational;
using Domain.Definition.EntityDomain.Component;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;

namespace Infrastructure.Repository.Relational
{
    public class AIDefinitionRepository : SQLGenericRepository<AIDefinition>, IAIDefinitionRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public AIDefinitionRepository(RelationalDB context) : base(context) { }

        #region Methods
        #endregion
    }
}