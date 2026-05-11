using Application.Services.Abstraction.WorldService;
using Domain.Abstraction.World;
using Domain.Common;

namespace Application.Systems.Resolver
{
    public readonly struct CollisionRequest
    {
        public readonly string EntityId;
        public readonly CollisionBody Body;
        public readonly Vector2 DesiredPosition;

        public CollisionRequest(
            string entityId,
            CollisionBody body,
            Vector2 desiredPosition)
        {
            EntityId = entityId;
            Body = body;
            DesiredPosition = desiredPosition;
        }
    }

    public readonly struct CollisionResult
    {
        public Vector2 FinalPosition { get; init; }
        public bool BlockX { get; init; }
        public bool BlockY { get; init; }
        public bool IsBlocked => BlockX || BlockY;
        public int LayerZ { get; init; }
    }

    public class CollisionResolver
    {
        #region Attributes
        private readonly ICollisionService collisionService;
        private readonly IWorldQuery world;
        #endregion

        #region Properties
        #endregion

        public CollisionResolver(
            ICollisionService collisionService,
            IWorldQuery world)
        {
            this.collisionService = collisionService;
            this.world = world;
        }

        #region Methods
        public Dictionary<string, CollisionResult> ResolveBatch(
            List<CollisionRequest> requests)
        {
            var results = new Dictionary<string, CollisionResult>(requests.Count);

            foreach (var req in requests)
            {
                var collision = collisionService.QueryMovement(
                    req.Body,
                    req.DesiredPosition);

                var resolved = Resolve(req.Body, req.DesiredPosition, collision);

                results[req.EntityId] = new CollisionResult
                {
                    FinalPosition = resolved,
                    BlockX = collision.BlockX,
                    BlockY = collision.BlockY,
                    LayerZ = collision.LayerZ
                };
            }

            return results;
        }

        private Vector2 Resolve(
            CollisionBody self,
            Vector2 desired,
            CollisionContext collision)
        {
            var final = desired;

            if (collision.BlockX)
                final.X = self.Position.X;

            if (collision.BlockY)
                final.Y = self.Position.Y;

            return final;
        }
        #endregion
    }
}