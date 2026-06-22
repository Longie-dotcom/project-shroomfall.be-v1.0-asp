using Application.Interfaces.Repository.Relational;
using Domain.Definition.MetaDomain;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;

namespace Infrastructure.Repository.Relational
{
    public class EffectDefinitionRepository : SQLGenericRepository<EffectDefinition>, IEffectDefinitionRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public EffectDefinitionRepository(RelationalDB context) : base(context) { }

        #region Methods
        #endregion
    }
}