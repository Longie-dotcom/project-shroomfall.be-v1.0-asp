using Domain.Abstraction;

namespace Domain.Definition.EntityDomain.Component
{
    public class LifetimeDefinition : ComponentDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public float Lifetime { get; private set; }
        #endregion

        protected LifetimeDefinition() : base() { }

        public LifetimeDefinition(
            Guid id,
            string entityDefinitionId,
            float lifetime) : base(id, entityDefinitionId)
        {
            Lifetime = lifetime;
        }

        #region Methods
        #endregion
    }
}