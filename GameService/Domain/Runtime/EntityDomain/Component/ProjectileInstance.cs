using Domain.Abstraction;
using Domain.Common;

namespace Domain.Runtime.EntityDomain.Component
{
    public class ProjectileInstance : ComponentInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public float Velocity { get; }
        public Vector2 Direction { get; set; }
        public string? OnImpactSpawnEntityDefinitionID { get; }
        #endregion

        public ProjectileInstance(
            Guid definitionId,
            float velocity,
            string? onImpactSpawnEntityDefinitionID) : base(definitionId)
        {
            Velocity = velocity;
            OnImpactSpawnEntityDefinitionID = onImpactSpawnEntityDefinitionID;
            Direction = Vector2.Zero;
        }

        #region Methods
        #endregion
    }
}