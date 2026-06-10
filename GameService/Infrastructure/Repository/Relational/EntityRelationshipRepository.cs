using Application.Interfaces.Repository.Relational;
using Domain.Definition.EntityDomain;
using Infrastructure.Persistence;

namespace Infrastructure.Repository.Relational
{
    public class EntityRelationshipRepository : SQLGenericRepository<EntityRelationship>, IEntityRelationshipRepository, IRelationalRepository
    {
        #region Attributes
        private readonly RelationalDB context;
        #endregion

        #region Properties
        #endregion

        public EntityRelationshipRepository(
            RelationalDB context) : base(
                context)
        {
            this.context = context;
        }

        #region Methods
        #endregion
    }
}