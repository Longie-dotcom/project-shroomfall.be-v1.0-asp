using Application.Context;
using Application.Services.WorldService;
using Domain.Common;

namespace Application.Systems.Resolver
{
    public readonly struct CreatureContext
    {
        public readonly string EntityId;
        public readonly CollisionBody Body;
        public readonly Vector2 DesiredPosition;

        public CreatureContext(
            string entityId,
            CollisionBody body,
            Vector2 desiredPosition)
        {
            EntityId = entityId;
            Body = body;
            DesiredPosition = desiredPosition;
        }
    }

    public readonly struct CreatureResult
    {
        public Vector2 FinalPosition { get; init; }
        public int LayerZ { get; init; }
    }

    public class CreatureResolver
    {
        #region Attributes
        private readonly CollisionService collisionService;
        private readonly WorldContext worldContext;
        #endregion

        #region Properties
        #endregion

        public CreatureResolver(
            CollisionService collisionService,
            WorldContext worldContext)
        {
            this.collisionService = collisionService;
            this.worldContext = worldContext;
        }

        #region Methods
        public Dictionary<string, CreatureResult> Resolve(
            List<CreatureContext> requests)
        {
            var results = new Dictionary<string, CreatureResult>(requests.Count);

            foreach (var req in requests)
            {
                var roomSpatial = worldContext.GetRoom(req.Body.RoomSpatialID);
                if (roomSpatial != null)
                {
                    var collision = collisionService.QueryMovement(
                        req.Body,
                        req.DesiredPosition);

                    var resolved = Resolve(req.Body, req.DesiredPosition, collision);

                    results[req.EntityId] = new CreatureResult
                    {
                        FinalPosition = resolved,
                        LayerZ = collision.LayerZ
                    };
                }
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