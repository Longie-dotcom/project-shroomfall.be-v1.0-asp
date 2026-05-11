using Application.Services.Abstraction.WorldService;
using Application.Systems.Resolver;
using Domain.Abstraction.World;
using Domain.Common;
using Domain.Definition.AttributeDomain.Enum;
using Domain.Runtime.EntityDomain;

namespace Application.Systems.System
{
    public readonly struct MovementResult
    {
        public Vector2 Position { get; init; }
        public Vector2 SlideDirection { get; init; }
        public bool Blocked { get; init; }
    }

    public class MovementSystem
    {
        #region Attributes
        private readonly IWorldQuery worldQuery;
        #endregion

        #region Properties
        #endregion

        public MovementSystem(
            IWorldQuery worldQuery)
        {
            this.worldQuery = worldQuery;
        }

        #region Methods
        public void Update(float dt, List<CollisionRequest> requests)
        {
            foreach (var creature in worldQuery.GetAll<CreatureInstance>())
            {
                if (!creature.WantsToMove)
                    continue;

                float speed = creature.Characteristic.GetCore(AttributeType.MoveSpeed);

                var desired = creature.Position + creature.Direction * speed * dt;

                var body = new CollisionBody(
                    creature.ID,
                    creature.RoomSpatialID,
                    creature.LayerZ,
                    creature.Position,
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