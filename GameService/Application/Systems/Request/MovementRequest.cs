using Application.Context;
using Application.Services.WorldService;
using Application.Systems.Resolver;
using Domain.Definition.AttributeDomain.Enum;
using Domain.Runtime.EntityDomain;

namespace Application.Systems.Request
{
    public class MovementRequest
    {
        #region Attributes
        private readonly WorldContext worldContext;
        #endregion

        #region Properties
        #endregion

        public MovementRequest(
            WorldContext worldContext)
        {
            this.worldContext = worldContext;
        }

        #region Methods
        public void Update(
            float dt,
            List<CollisionRequest> requests)
        {
            foreach (var creature in worldContext.GetEntities<CreatureInstance>())
            {
                if (!creature.WantsToMove)
                    continue;

                // Resolve creature desired position
                float speed = creature.Characteristic.GetCore(AttributeType.MoveSpeed);
                var desired = creature.Position + creature.Direction * speed * dt;

                // Request for collision resolving
                var body = new CollisionBody(
                    creature.ID,
                    creature.RoomSpatialID,
                    creature.Position,
                    creature.LayerZ,
                    creature.CollisionShape);

                requests.Add(new CollisionRequest(
                    creature.ID,
                    body,
                    desired));
            }
        }
        #endregion
    }
}