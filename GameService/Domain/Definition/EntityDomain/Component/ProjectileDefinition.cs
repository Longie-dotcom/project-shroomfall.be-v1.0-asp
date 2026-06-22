using Domain.Abstraction;

namespace Domain.Definition.EntityDomain.Component
{
    public class ProjectileDefinition : ComponentDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public float Velocity { get; private set; }
        public string? OnImpactSpawnEntityDefinitionID { get; private set; }
        #endregion

        protected ProjectileDefinition() : base() { }

        public ProjectileDefinition(
            Guid id,
            string entityDefinitionId,
            float velocity) : base(id, entityDefinitionId)
        {
            Velocity = velocity;
        }

        #region Methods
        #endregion
    }
}