using Domain.Abstraction;

namespace Domain.Definition.EntityDomain.Component
{
    public class LifetimeDefinition : ComponentDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public float Duration { get; private set; }
        #endregion

        protected LifetimeDefinition() : base() { }

        public LifetimeDefinition(
            Guid id,
            string entityDefinitionId,
            float duration) : base(id, entityDefinitionId)
        {
            Duration = duration;
        }

        #region Methods
        #endregion
    }
}