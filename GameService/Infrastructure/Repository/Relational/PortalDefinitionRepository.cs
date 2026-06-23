using Application.Interfaces.Repository.Relational;
using Domain.Definition.EntityDomain.Component;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;

namespace Infrastructure.Repository.Relational
{
    public class PortalDefinitionRepository : SQLDefinitionRepository<PortalDefinition>, IPortalDefinitionRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public PortalDefinitionRepository(RelationalDB context) : base(context) { }

        #region Methods
        #endregion
    }
}