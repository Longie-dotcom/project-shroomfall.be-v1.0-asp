using Contract;
using Contract.Enum.EntityDomain;
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
        public Vector2 CollisionOffset { get; }
        public string RoomSpatialID { get; protected set; }
        public int LayerZ { get; protected set; }
        public Vector2 Position { get; protected set; }
        public bool PositionChangedThisFrame { get; private set; }
        public bool WantsToMove { get; private set; }
        public Vector2 MovementVector { get; protected set; }
        public EntityDirection FacingDirection { get; protected set; }
        public EntityAction CurrentAction { get; protected set; }
        public bool IsActionLocked { get; private set; }
        public AppearanceInstance Appearance { get; protected set; }
        #endregion

        protected EntityInstance(
            string id,
            string definitionId,
            ICollisionShape collisionShape,
            Vector2 collisionOffset,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 movementVector,
            AppearanceInstance appearance)
        {
            ID = id;
            DefinitionID = definitionId;
            CollisionShape = collisionShape;
            CollisionOffset = collisionOffset;
            RoomSpatialID = roomSpatialId;
            LayerZ = layerZ;
            Position = position;
            WantsToMove = false;
            MovementVector = movementVector;
            FacingDirection = EntityDirection.DOWN;
            CurrentAction = EntityAction.IDLE;
            Appearance = appearance;
        }

        #region Methods
        public void SetPosition(Vector2 position, int layerZ)
        {
            // Check if the new position actually shifts away from old coordinates
            PositionChangedThisFrame = (Position - position).LengthSquared() > 0.0001f;

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

        public void SetMovementIntent(
            Vector2 inputVector)
        {
            if (IsActionLocked) return;

            // Check if the player cleared their inputs (Stopped moving)
            if (inputVector.LengthSquared() < 0.0001f)
            {
                MovementVector = Vector2.Zero;
                WantsToMove = false;
                CurrentAction = EntityAction.IDLE;
                // Note: FacingDirection is purposefully NOT updated here, preserving their last look vector!
                return;
            }

            // Normalize and apply physics values
            if (inputVector.LengthSquared() > 1f)
                inputVector.Normalize(); // Ensure custom client calculations don't speed-hack

            MovementVector = Vector2.Normalize(inputVector);
            WantsToMove = true;
            CurrentAction = EntityAction.RUN;

            // Server-side Direction Derivation
            FacingDirection = Vector2ToDirection(MovementVector, FacingDirection);
        }

        public void ExecuteAction(
            EntityAction action, 
            float lockDuration)
        {
            CurrentAction = action;
            MovementVector = Vector2.Zero;
            WantsToMove = false;

            if (lockDuration > 0f)
            {
                IsActionLocked = true;
            }
        }

        public void UnlockAction()
        {
            IsActionLocked = false;
            CurrentAction = EntityAction.IDLE;
        }

        public (int cx, int cy, int x, int y, int z) GetSpatialKey()
        {
            int wx = (int)MathF.Floor(Position.X);
            int wy = (int)MathF.Floor(Position.Y);

            var (cx, cy, x, y) = ChunkMath.ToChunk(wx, wy, Constraint.CHUNK_SIZE);

            return (cx, cy, x, y, LayerZ);
        }

        private EntityDirection Vector2ToDirection(
            Vector2 vector,
            EntityDirection fallback)
        {
            if (vector.LengthSquared() < 0.0001f) return fallback;

            // Compare absolute sizes of X and Y to find dominant direction thread
            if (MathF.Abs(vector.Y) >= MathF.Abs(vector.X))
            {
                return vector.Y > 0 ? EntityDirection.UP : EntityDirection.DOWN;
            }
            else
            {
                return vector.X > 0 ? EntityDirection.RIGHT : EntityDirection.LEFT;
            }
        }
        #endregion
    }
}