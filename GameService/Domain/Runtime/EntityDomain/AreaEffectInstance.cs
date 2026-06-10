using Domain.Abstraction;
using Domain.Common;
using Domain.Runtime.EntityDomain.Component;

namespace Domain.Runtime.EntityDomain
{
    public class AreaEffectInstance : EntityInstance
    {
        #region Attributes
        public string EntityInstanceOwnerID { get; }
        public string SourceDefinitionID { get; }
        public float Duration { get; }
        #endregion

        #region Properties
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
            string sourceDefinitionId,
            float duration) : base(
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
        }

        #region Methods
        #endregion
    }
}