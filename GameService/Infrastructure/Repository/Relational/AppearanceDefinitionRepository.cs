using Application.Interfaces.Repository.Relational;
using Domain.Definition.EntityDomain.Component;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;

namespace Infrastructure.Repository.Relational
{
    public class AppearanceDefinitionRepository : SQLGenericRepository<AppearanceDefinition>, IAppearanceDefinitionRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public AppearanceDefinitionRepository(RelationalDB context) : base(context) { }

        #region Methods
        #endregion
    }
}