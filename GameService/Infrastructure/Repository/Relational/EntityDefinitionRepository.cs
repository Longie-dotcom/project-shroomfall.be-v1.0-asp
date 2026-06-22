using Application.Interfaces.Repository.Relational;
using Domain.Definition.EntityDomain;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;

namespace Infrastructure.Repository.Relational
{
    public class EntityDefinitionRepository : SQLGenericRepository<EntityDefinition>, IEntityDefinitionRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public EntityDefinitionRepository(RelationalDB context) : base(context) { }

        #region Methods
        #endregion
    }
}