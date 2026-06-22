using Application.Context;
using Application.Interfaces.Cache;
using Contract.Enum.EntityDomain;
using Contract.Enum.WorldDomain;
using Domain.Abstraction;
using Domain.Common;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.WorldDomain.Spatial;

namespace Application.Services.WorldService
{
    public readonly struct CollisionBody
    {
        public string EntityInstanceID { get; }
        public string RoomSpatialID { get; }
        public Vector2 Position { get; }
        public Vector2 Offset { get; }
        public int LayerZ { get; }
        public ICollisionShape CollisionShape { get; }
        public CollisionLayer Layer { get; }
        public CollisionLayer Mask { get; }

        public CollisionBody(
            string entityInstanceID,
            string roomSpatialID,
            Vector2 position,
            Vector2 offset,
            int layerZ,
            ICollisionShape collisionShape,
            CollisionLayer layer,
            CollisionLayer mask)
        {
            EntityInstanceID = entityInstanceID;
            RoomSpatialID = roomSpatialID;
            Position = position;
            Offset = offset;
            LayerZ = layerZ;
            CollisionShape = collisionShape;
            Layer = layer;
            Mask = mask;
        }
    }

    public class CollisionContext
    {
        public bool IsBlocked { get; set; }
        public bool BlockX { get; set; }
        public bool BlockY { get; set; }
        public int LayerZ { get; set; }
        public List<EntityInstance> Entities { get; } // Dynamic collisions
        public HashSet<EntityInstance> Triggers { get; } // Trigger entities

        public CollisionContext()
        {
            Entities = new List<EntityInstance>();
            Triggers = new HashSet<EntityInstance>();
        }
    }

    public class CollisionService
    {
        #region Attributes
        private readonly ICacheProvider cacheProvider;
        private readonly WorldContext worldContext;
        #endregion

        #region Properties
        #endregion

        public CollisionService(
            ICacheProvider cacheProvider,
            WorldContext worldContext)
        {
            this.cacheProvider = cacheProvider;
            this.worldContext = worldContext;
        }

        #region Methods
        public CollisionContext QueryMovement(
            CollisionBody self,
            Vector2 desiredPosition)
        {
            var result = new CollisionContext();

            var roomSpatial = worldContext.GetRoom(self.RoomSpatialID);
            if (roomSpatial == null) return result;

            Span<(int x, int y)> buffer = stackalloc (int, int)[256];

            result.BlockX = QueryInteractions(self, roomSpatial, new Vector2(desiredPosition.X, self.Position.Y), result, buffer);
            result.BlockY = QueryInteractions(self, roomSpatial, new Vector2(self.Position.X, desiredPosition.Y), result, buffer);
            result.IsBlocked = result.BlockX || result.BlockY;
            result.LayerZ = ResolveLayer(self, roomSpatial, desiredPosition);

            return result;
        }

        public CollisionContext QueryOverlap(
            CollisionBody self,
            Vector2 position)
        {
            var result = new CollisionContext();

            var roomSpatial = worldContext.GetRoom(self.RoomSpatialID);
            if (roomSpatial == null) return result;

            Span<(int x, int y)> buffer = stackalloc (int, int)[256];

            result.IsBlocked = QueryInteractions(self, roomSpatial, position, result, buffer);

            return result;
        }

        public void SpawnAtNearestValidPosition(
            EntityInstance entity,
            string roomDefinitionId,
            string roomSpatialId,
            Vector2 targetPosition,
            int targetLayerZ,
            IEnumerable<EntityInstance>? pendingEntities = null,
            int maxRadius = 5)
        {
            // Default to the target position
            Vector2 finalPosition = targetPosition;

            // If collision exists, find the nearest valid spot
            var collision = entity.GetComponent<CollisionInstance>();
            if (collision != null && collision.CollisionShape.IsBlocking)
            {
                finalPosition = FindNearestValid(
                    collision.CollisionShape,
                    collision.Mask,
                    roomDefinitionId,
                    roomSpatialId,
                    targetPosition,
                    collision.CollisionOffset,
                    targetLayerZ,
                    entity.ID,
                    pendingEntities,
                    maxRadius);
            }

            // Apply the final position to the entity's transform
            var transform = entity.GetComponent<TransformInstance>();
            if (transform != null)
            {
                transform.SetPosition(finalPosition, targetLayerZ);
            }
        }

        private Vector2 FindNearestValid(
            ICollisionShape shape,
            CollisionLayer mask,
            string roomDefId,
            string roomSpatialId,
            Vector2 pos,
            Vector2 offset,
            int layerZ,
            string? ignoreEntityId,
            IEnumerable<EntityInstance>? pendingEntities = null,
            int maxRadius = 5)
        {
            // Check initial
            if (IsValidPosition(shape, mask, roomDefId, roomSpatialId, pos, offset, layerZ, ignoreEntityId, pendingEntities))
                return pos;

            // Spiral search logic
            for (int r = 1; r <= maxRadius; r++)
            {
                for (int dx = -r; dx <= r; dx++)
                {
                    for (int dy = -r; dy <= r; dy++)
                    {
                        if (Math.Abs(dx) != r && Math.Abs(dy) != r) continue;
                        Vector2 candidate = new Vector2(pos.X + dx, pos.Y + dy);
                        if (IsValidPosition(shape, mask, roomDefId, roomSpatialId, candidate, offset, layerZ, ignoreEntityId, pendingEntities))
                            return candidate;
                    }
                }
            }
            return pos;
        }

        private bool IsValidPosition(
            ICollisionShape shape,
            CollisionLayer mask,
            string roomDefinitionId,
            string roomSpatialId,
            Vector2 position,
            Vector2 offset,
            int layerZ,
            string? ignoreEntityId,
            IEnumerable<EntityInstance>? pendingEntities = null)
        {
            Span<(int x, int y)> buffer = stackalloc (int, int)[256];
            Vector2 effectivePos = new Vector2(position.X + offset.X, position.Y + offset.Y);
            int count = shape.ComputeCells(effectivePos, buffer);

            HashSet<string> checkedEntities = new HashSet<string>();

            for (int i = 0; i < count; i++)
            {
                var (cellX, cellY) = buffer[i];

                var cell = cacheProvider.Room.GetTopCell(roomDefinitionId, cellX, cellY);
                if (cell != null && cell.Type != CellType.Walkable) return false;

                var roomSpatial = worldContext.GetRoom(roomSpatialId);
                if (roomSpatial != null)
                {
                    var (_, entities) = worldContext.QuerySpatial(roomSpatialId, cellX, cellY, layerZ);
                    foreach (var entity in entities)
                    {
                        if (entity.ID == ignoreEntityId) continue;

                        // Deduplicate entity checks
                        if (!checkedEntities.Add(entity.ID)) continue;

                        if (IsCollidingWithEntityInstance(shape, mask, effectivePos, entity)) return false;
                    }
                }

                if (pendingEntities != null)
                {
                    foreach (var entity in pendingEntities)
                    {
                        if (entity.ID == ignoreEntityId) continue;

                        if (!checkedEntities.Add(entity.ID)) continue;

                        if (IsCollidingWithEntityInstance(shape, mask, effectivePos, entity)) return false;
                    }
                }
            }
            return true;
        }

        private bool IsCollidingWithEntityInstance(
            ICollisionShape selfShape,
            CollisionLayer selfMask,
            Vector2 selfPos,
            EntityInstance entity)
        {
            var collision = entity.GetComponent<CollisionInstance>();
            var transform = entity.GetComponent<TransformInstance>();
            if (collision == null || transform == null || !collision.CollisionShape.IsBlocking) return false;

            // --- THE COLLISION MATRIX CHECK ---
            if ((selfMask & collision.Layer) == 0) return false;

            Vector2 effectiveEntityPos = new Vector2(
                transform.Position.X + collision.CollisionOffset.X,
                transform.Position.Y + collision.CollisionOffset.Y);

            return selfShape.Intersects(selfPos, collision.CollisionShape, effectiveEntityPos);
        }

        private bool QueryInteractions(
            CollisionBody self,
            RoomSpatial roomSpatial,
            Vector2 position,
            CollisionContext context,
            Span<(int x, int y)> buffer)
        {
            bool isBlocked = false;

            // THREAD SAFETY: Local stack buffer instead of shared class field
            Span<(int x, int y)> visitedBuffer = stackalloc (int, int)[16];
            int visitedCount = 0;

            Vector2 effectiveSelfPosition = new Vector2(
                position.X + self.Offset.X,
                position.Y + self.Offset.Y);

            int count = self.CollisionShape.ComputeCells(effectiveSelfPosition, buffer);

            for (int i = 0; i < count; i++)
            {
                var cell = buffer[i];

                // Manual inline check for visited cells (super fast on stack)
                bool alreadyVisited = false;
                for (int j = 0; j < visitedCount; j++)
                {
                    if (visitedBuffer[j] == cell)
                    {
                        alreadyVisited = true;
                        break;
                    }
                }

                if (alreadyVisited) continue;

                // Record visited cell
                if (visitedCount < visitedBuffer.Length)
                {
                    visitedBuffer[visitedCount++] = cell;
                }

                var (_, entities) = worldContext.QuerySpatial(self.RoomSpatialID, cell.x, cell.y, self.LayerZ);

                foreach (var entity in entities)
                {
                    if (entity == null || entity.ID == self.EntityInstanceID) continue;

                    var collision = entity.GetComponent<CollisionInstance>();
                    if (collision == null) continue;

                    // --- THE COLLISION MATRIX CHECK ---
                    if ((self.Mask & collision.Layer) == 0) continue;

                    var transform = entity.GetComponent<TransformInstance>();
                    if (transform == null) continue;

                    Vector2 effectiveEntityPosition = new Vector2(
                        transform.Position.X + collision.CollisionOffset.X,
                        transform.Position.Y + collision.CollisionOffset.Y);

                    if (self.CollisionShape.Intersects(effectiveSelfPosition, collision.CollisionShape, effectiveEntityPosition))
                    {
                        if (context.Triggers.Add(entity))
                        {
                            context.Entities.Add(entity);
                        }

                        if (collision.CollisionShape.IsBlocking) isBlocked = true;
                    }
                }

                var tileCell = cacheProvider.Room.GetTopCell(roomSpatial.DefinitionID, cell.x, cell.y);
                if (tileCell != null && tileCell.Type != CellType.Walkable) isBlocked = true;
            }
            return isBlocked;
        }

        private int ResolveLayer(
            CollisionBody self,
            RoomSpatial roomSpatial,
            Vector2 desiredPosition)
        {
            int wx = (int)MathF.Floor(desiredPosition.X);
            int wy = (int)MathF.Floor(desiredPosition.Y);

            var cell = cacheProvider.Room.GetTopCell(roomSpatial.DefinitionID, wx, wy);

            return cell?.Z ?? self.LayerZ;
        }
        #endregion
    }
}