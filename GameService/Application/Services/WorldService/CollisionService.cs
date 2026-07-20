using Application.Interfaces.Cache;
using Contract.Enum.EntityDomain;
using Contract.Enum.WorldDomain;
using Domain.Abstraction;
using Domain.Common;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.WorldDomain;
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
        #region Movement and Interaction
        /// <summary>
        /// Simulates an entity's movement toward a desired position, evaluating potential environmental 
        /// and entity collisions independently across the X and Y axes to support smooth sliding behavior.
        /// Also determines if the movement results in a vertical layer (Z-index) transition.
        /// </summary>
        /// <param name="self">The snapshot body representation of the moving entity.</param>
        /// <param name="desiredPosition">The intended target coordinates for the movement step.</param>
        /// <returns>A <see cref="CollisionContext"/> detailing axis blocks, intersecting entities, and the resolved Z-layer.</returns>
        public CollisionContext QueryMovement(
            CollisionBody self,
            Vector2 desiredPosition)
        {
            var result = new CollisionContext();
            Console.WriteLine(
$"Checking collision at {desiredPosition.X},{desiredPosition.Y}, {self.Mask.ToString()}");

            var roomSpatial = worldContext.GetRoom(self.RoomSpatialID);
            if (roomSpatial == null) return result;

            Span<(int x, int y)> buffer = stackalloc (int, int)[256];

            result.BlockX = QueryInteractions(self, roomSpatial, new Vector2(desiredPosition.X, self.Position.Y), result, buffer);
            result.BlockY = QueryInteractions(self, roomSpatial, new Vector2(self.Position.X, desiredPosition.Y), result, buffer);
            result.IsBlocked = result.BlockX || result.BlockY;
            result.LayerZ = ResolveLayer(self, roomSpatial, desiredPosition);

            return result;
        }

        /// <summary>
        /// Performs a static spatial intersection query at a specific position to find all overlapping 
        /// entity colliders or blocking tile environments without simulating movement steps.
        /// </summary>
        /// <param name="self">The snapshot body representation of the entity executing the query.</param>
        /// <param name="position">The exact coordinates where the overlap shape should be checked.</param>
        /// <returns>A <see cref="CollisionContext"/> containing the blocked state and lists of all intersecting triggers or entities.</returns>
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

        #region Spawning
        /// <summary>
        /// Spawns an entity using its structural physics snapshot, resolving the nearest valid 
        /// unblocked position via spiral search if the initial target coordinates result in a collision.
        /// </summary>
        /// <param name="self">The structural snapshot body representing the entity's physics properties.</param>
        /// <param name="transform">The transform instance of the spawning entity, updated with the resolved position.</param>
        /// <param name="roomDefinitionId">The static configuration/tile template ID of the room.</param>
        /// <param name="pendingEntities">Optional collection of staging/not-yet-spawned entities to validate against.</param>
        /// <param name="maxRadius">The maximum grid distance to search outward if the original position is blocked.</param>
        public void SpawnAtNearestValidPosition(
            CollisionBody self,
            TransformInstance transform,
            string roomDefinitionId,
            IEnumerable<EntityInstance>? pendingEntities = null,
            int maxRadius = 5)
        {
            // If the shape isn't blocking, skip the search entirely and spawn directly
            Vector2 finalPosition = self.Position;

            if (self.CollisionShape.IsBlocking)
            {
                finalPosition = FindNearestValid(
                    self,
                    roomDefinitionId,
                    pendingEntities,
                    maxRadius);
            }

            transform.SetPosition(finalPosition, self.LayerZ);
        }

        private Vector2 FindNearestValid(
            CollisionBody self,
            string roomDefId,
            IEnumerable<EntityInstance>? pendingEntities = null,
            int maxRadius = 5)
        {
            // Cache room object
            var existedRoom = worldContext.GetRoom(self.RoomSpatialID);

            if (IsValidPosition(self, existedRoom, roomDefId, pendingEntities))
                return self.Position;

            for (int r = 1; r <= maxRadius; r++)
            {
                for (int dx = -r; dx <= r; dx++)
                {
                    for (int dy = -r; dy <= r; dy++)
                    {
                        if (Math.Abs(dx) != r && Math.Abs(dy) != r) continue;

                        // Construct a new structural snapshot for the hypothetical candidate position
                        var candidateBody = new CollisionBody(
                            self.EntityInstanceID,
                            self.RoomSpatialID,
                            new Vector2(self.Position.X + dx, self.Position.Y + dy),
                            self.Offset,
                            self.LayerZ,
                            self.CollisionShape,
                            self.Layer,
                            self.Mask
                        );

                        if (IsValidPosition(candidateBody, existedRoom, roomDefId, pendingEntities))
                            return candidateBody.Position;
                    }
                }
            }
            return self.Position;
        }

        private bool IsValidPosition(
            CollisionBody self,
            RoomSpatial? existedRoom,
            string roomDefinitionId,
            IEnumerable<EntityInstance>? pendingEntities = null)
        {
            Span<(int x, int y)> buffer = stackalloc (int, int)[256];

            Vector2 effectivePos = new Vector2(self.Position.X + self.Offset.X, self.Position.Y + self.Offset.Y);
            int count = self.CollisionShape.ComputeCells(effectivePos, buffer);

            HashSet<string> checkedEntities = new HashSet<string>();

            for (int i = 0; i < count; i++)
            {
                var (cellX, cellY) = buffer[i];

                var cell = cacheProvider.Room.GetTopCell(roomDefinitionId, cellX, cellY);
                if (cell != null && cell.Type != CellType.Walkable) return false;

                // For existed room - direct query on the object instead of worldContext
                if (existedRoom != null)
                {
                    foreach (var entity in existedRoom.Query(cellX, cellY, self.LayerZ))
                    {
                        if (entity.ID == self.EntityInstanceID) continue;
                        if (!checkedEntities.Add(entity.ID)) continue;

                        if (IsCollidingWithEntityInstance(self, effectivePos, entity)) return false;
                    }
                }

                // For non existed room (new room batch initialization)
                if (pendingEntities != null)
                {
                    foreach (var entity in pendingEntities)
                    {
                        if (entity.ID == self.EntityInstanceID) continue;
                        if (!checkedEntities.Add(entity.ID)) continue;

                        if (IsCollidingWithEntityInstance(self, effectivePos, entity)) return false;
                    }
                }
            }

            return true;
        }

        private bool IsCollidingWithEntityInstance(
            CollisionBody self,
            Vector2 selfEffectivePos,
            EntityInstance targetEntity)
        {
            var targetCollision = targetEntity.GetComponent<CollisionInstance>();
            var targetTransform = targetEntity.GetComponent<TransformInstance>();

            if (targetCollision == null || targetTransform == null || !targetCollision.CollisionShape.IsBlocking)
                return false;

            // --- THE COLLISION MATRIX CHECK ---
            if ((self.Mask & targetCollision.Layer) == 0) return false;

            Vector2 targetEffectivePos = new Vector2(
                targetTransform.Position.X + targetCollision.CollisionOffset.X,
                targetTransform.Position.Y + targetCollision.CollisionOffset.Y);

            return self.CollisionShape.Intersects(
                selfEffectivePos,
                targetCollision.CollisionShape,
                targetEffectivePos);
        }
        #endregion
        #endregion
    }
}