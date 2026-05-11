using Domain.Abstraction;
using Domain.Common;
using Domain.Runtime.EntityDomain.Component;
using Domain.Shared;

namespace Domain.Runtime.EntityDomain
{
    public class EntityInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; }
        public string DefinitionID { get; }
        public ICollisionShape CollisionShape { get; }
        public string RoomSpatialID { get; protected set; }
        public int LayerZ { get; protected set; }
        public Vector2 Position { get; protected set; }
        public bool WantsToMove { get; private set; }
        public Vector2 Direction { get; protected set; }
        public AppearanceInstance Appearance { get; protected set; }
        #endregion

        protected EntityInstance(
            string id,
            string definitionId,
            ICollisionShape collisionShape,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 direction,
            AppearanceInstance appearance)
        {
            ID = id;
            DefinitionID = definitionId;
            CollisionShape = collisionShape;
            RoomSpatialID = roomSpatialId;
            LayerZ = layerZ;
            Position = position;
            WantsToMove = false;
            Direction = direction;
            Appearance = appearance;
        }

        #region Methods
        public void SetPosition(
            Vector2 position, 
            int layerZ)
        {
            Position = position;
            LayerZ = layerZ;
        }

        public void ChangeRoom(
            string roomSpatialId,
            Vector2 position,
            int layerZ)
        {
            RoomSpatialID = roomSpatialId;
            LayerZ = layerZ;
            Position = position;
        }

        public void SetMovementIntent(Vector2 direction)
        {
            if (direction.LengthSquared() < 0.0001f)
            {
                Direction = Vector2.Zero;
                WantsToMove = false;
                return;
            }

            if (direction.LengthSquared() > 1f)
                direction.Normalize();

            Direction = Vector2.Normalize(direction);
            WantsToMove = true;
        }

        public (int cx, int cy, int x, int y, int z) GetSpatialKey()
        {
            int wx = (int)MathF.Floor(Position.X);
            int wy = (int)MathF.Floor(Position.Y);

            var (cx, cy, x, y) = ChunkMath.ToChunk(wx, wy, Constraint.CHUNK_SIZE);

            return (cx, cy, x, y, LayerZ);
        }
        #endregion
    }
}