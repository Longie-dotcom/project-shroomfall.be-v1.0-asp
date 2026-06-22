using Application.Interfaces.Repository.Relational;
using Domain.Definition.EntityDomain.Component;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;

namespace Infrastructure.Repository.Relational
{
    public class InteractableDefinitionRepository : SQLGenericRepository<InteractableDefinition>, IInteractableDefinitionRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public InteractableDefinitionRepository(RelationalDB context) : base(context) { }

        #region Methods
        #endregion
    }
}