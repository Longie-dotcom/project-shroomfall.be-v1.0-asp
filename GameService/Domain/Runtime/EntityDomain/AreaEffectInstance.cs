using Domain.Abstraction;
using Domain.Common;
using Domain.Runtime.EntityDomain.Component;

namespace Domain.Runtime.EntityDomain
{
    public class AreaEffectInstance : EntityInstance
    {
        #region Attributes
        private float elapsedLifetime;
        private float tickAccumulator;
        #endregion

        #region Properties
        public string EntityInstanceOwnerID { get; }
        public string? SourceDefinitionID { get; }
        public float Duration { get; }
        public float TickInterval { get; }
        #endregion

        public AreaEffectInstance(
            string id,
            string definitionId,
            ICollisionShape collisionShape,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 movementVector,
            AppearanceInstance appearance,
            string entityInstanceOwnerId,
            string? sourceDefinitionId,
            float duration,
            float tickInterval) : base(
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
            TickInterval = tickInterval;
            tickAccumulator = tickInterval;
        }

        #region Methods
        public void TickLifetime(
            float dt)
        {
            elapsedLifetime += dt;
            tickAccumulator += dt;
        }

        public bool IsExpired()
        {
            return elapsedLifetime >= Duration;
        }

        public bool CanTickThisFrame()
        {
            if (tickAccumulator >= TickInterval)
            {
                // Subtract instead of setting to 0 to preserve leftover frame deltas (prevents clock drift)
                tickAccumulator -= TickInterval;
                return true;
            }

            return false;
        }
        #endregion
    }
}