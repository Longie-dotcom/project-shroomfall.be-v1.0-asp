using Application.Interfaces.Repository.Relational;
using Domain.Definition.EntityDomain;
using Infrastructure.Persistence;

namespace Infrastructure.Repository.Relational
{
    public class EntityRepository : SQLGenericRepository<Entity>, IEntityRepository, IRelationalRepository
    {
        #region Attributes
        private readonly RelationalDB context;
        #endregion

        #region Properties
        #endregion

        public EntityRepository(
            RelationalDB context) : base(
                context)
        {
            this.context = context;
        }

        #region Methods
        #endregion
    }
}