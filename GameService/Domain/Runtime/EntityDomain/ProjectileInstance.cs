using Contract.Enum.EntityDomain;
using Domain.Abstraction;
using Domain.Common;
using Domain.Runtime.EntityDomain.Component;

namespace Domain.Runtime.EntityDomain
{
    public class ProjectileInstance : EntityInstance
    {
        #region Attributes
        private float elapsedLifetime;
        #endregion

        #region Properties
        public string EntityInstanceOwnerID { get; }
        public string SourceDefinitionID { get; }
        public float Duration { get; }
        public float Velocity { get; }
        public IReadOnlyDictionary<EntityRelationshipType, List<string>> Relationships { get; }
        #endregion

        public ProjectileInstance(
            string id,
            string definitionId,
            ICollisionShape collisionShape,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 movementVector,
            AppearanceInstance appearance,
            string entityInstanceOwnerId,
            string sourceDefinitionId,
            float duration,
            float velocity,
            IReadOnlyDictionary<EntityRelationshipType, List<string>> relationships) : base(
                id,
                definitionId,
                collisionShape,
                roomSpatialId,
                layerZ,
                position,
                movementVector,
                appearance)
        {
            EntityInstanceOwnerID = entityInstanceOwnerId;
            SourceDefinitionID = sourceDefinitionId;
            Duration = duration;
            elapsedLifetime = 0f;
            Velocity = velocity;
            Relationships = relationships;
        }

        #region Methods
        public void TickLifetime(
            float dt)
        {
            elapsedLifetime += dt;
        }

        public bool IsExpired()
        {
            return elapsedLifetime >= Duration;
        }
        #endregion
    }
}