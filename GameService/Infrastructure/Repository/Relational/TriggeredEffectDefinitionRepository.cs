using Application.Interfaces.Repository.Relational;
using Domain.Definition.EntityDomain.Component;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;

namespace Infrastructure.Repository.Relational
{
    public class TriggeredEffectDefinitionRepository : SQLGenericRepository<TriggeredEffectDefinition>, ITriggeredEffectDefinitionRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public TriggeredEffectDefinitionRepository(RelationalDB context) : base(context) { }

        #region Methods
        #endregion
    }
}