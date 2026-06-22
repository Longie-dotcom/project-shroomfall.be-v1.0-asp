using Application.Interfaces.Repository.Relational;
using Domain.Definition.EntityDomain.Component;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;

namespace Infrastructure.Repository.Relational
{
    public class EntityRelationshipDefinitionRepository : SQLGenericRepository<EntityRelationshipDefinition>, IEntityRelationshipDefinitionRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public EntityRelationshipDefinitionRepository(RelationalDB context) : base(context) { }

        #region Methods
        #endregion
    }
}