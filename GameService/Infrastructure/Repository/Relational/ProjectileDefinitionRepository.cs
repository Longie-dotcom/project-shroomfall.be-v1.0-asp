using Application.Interfaces.Repository.Relational;
using Domain.Definition.EntityDomain.Component;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;

namespace Infrastructure.Repository.Relational
{
    public class ProjectileDefinitionRepository : SQLGenericRepository<ProjectileDefinition>, IProjectileDefinitionRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public ProjectileDefinitionRepository(RelationalDB context) : base(context) { }

        #region Methods
        #endregion
    }
}