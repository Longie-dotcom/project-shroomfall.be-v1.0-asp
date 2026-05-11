using Domain.Abstraction;
using Domain.Common;
using Domain.Runtime.EntityDomain;

namespace Application.Services.Abstraction.WorldService
{
    public readonly struct CollisionBody
    {
        public string EntityInstanceID { get; }
        public string RoomSpatialID { get; }
        public int LayerZ { get; }
        public Vector2 Position { get; }
        public ICollisionShape CollisionShape { get; }

        public CollisionBody(
            string entityInstanceId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            ICollisionShape shape)
        {
            EntityInstanceID = entityInstanceId;
            RoomSpatialID = roomSpatialId;
            LayerZ = layerZ;
            Position = position;
            CollisionShape = shape;
        }
    }

    public class CollisionContext
    {
        public bool BlockX { get; set; }
        public bool BlockY { get; set; }
        public bool IsBlocked { get; set; }
        public int LayerZ { get; set; }
        public List<EntityInstance> Entities { get; } = new();
        public List<string> Triggers { get; } = new();
    }

    public interface ICollisionService
    {
        CollisionContext QueryMovement(
            CollisionBody self,
            Vector2 desiredPosition);
        CollisionContext QueryPoint(
            ICollisionShape shape,
            string roomSpatialId,
            Vector2 position,
            int layerZ);
    }
}
