using Domain.Runtime.EntityDomain;

namespace Domain.Abstraction
{
    public abstract class ComponentInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid DefinitionID { get; protected set; }
        public EntityInstance Entity { get; private set; } = new EntityInstance("", "");
        #endregion

        protected ComponentInstance(
            Guid definitionId)
        {
            DefinitionID = definitionId;
        }

        #region Methods
        public void Attach(
            EntityInstance entity)
        {
            Entity = entity;
        }
        #endregion
    }
}