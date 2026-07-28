using Contract;
using Contract.Enum.EntityDomain;
using Domain.Abstraction;
using Domain.Common;
using Domain.Shared;

namespace Domain.Runtime.EntityDomain.Component
{
    public class TransformInstance : ComponentInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public string RoomSpatialID { get; private set; }
        public int LayerZ { get; private set; }
        public Vector2 Position { get; private set; }
        public EntityDirection FacingDirection { get; private set; }
        public EntityAction CurrentAction { get; private set; }

        public bool WantsToMove { get; private set; }
        public Vector2 MovementVector { get; private set; }
        public bool PositionChangedThisFrame { get; private set; }
        public bool IsActionLocked { get; private set; }
        public bool NeedsActionSync { get; private set; }
        #endregion

        public TransformInstance(
            string roomSpatialId,
            int layerZ,
            Vector2 position) : base(Guid.Empty)
        {
            RoomSpatialID = roomSpatialId;
            LayerZ = layerZ;
            Position = position;
            FacingDirection = EntityDirection.DOWN;
            CurrentAction = EntityAction.IDLE;

            WantsToMove = false;
            MovementVector = Vector2.Zero;
        }

        #region Methods
        public void SetMovementIntent(
            Vector2 inputVector)
        {
            if (IsActionLocked) return;

            // Check if the player cleared their inputs (Stopped moving)
            if (inputVector.LengthSquared() < 0.0001f)
            {
                if (WantsToMove || CurrentAction != EntityAction.IDLE)
                {
                    NeedsActionSync = true;
                }

                MovementVector = Vector2.Zero;
                WantsToMove = false;
                CurrentAction = EntityAction.IDLE;
                return;
            }

            // Normalize and apply physics values
            if (inputVector.LengthSquared() > 1f)
                inputVector.Normalize();

            MovementVector = Vector2.Normalize(inputVector);
            WantsToMove = true;
            CurrentAction = EntityAction.RUN;

            // Server-side Direction Derivation
            FacingDirection = Vector2ToDirection(MovementVector, FacingDirection);
        }

        public void ClearMovementIntent()
        {
            SetMovementIntent(Vector2.Zero);
        }

        public void ClearActionSync()
        {
            NeedsActionSync = false;
        }

        public void SetPosition(
            Vector2 position,
            int layerZ)
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