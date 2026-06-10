using Application.Context;
using Application.Interfaces.Cache;
using Contract;
using Contract.Enum.WorldDomain;
using Domain.Abstraction;
using Domain.Common;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.WorldDomain;
using Domain.Shared;

namespace Application.Services.WorldService
{
    public readonly struct CollisionBody
    {
        public string EntityInstanceID { get; }
        public string RoomSpatialID { get; }
        public Vector2 Position { get; }
        public int LayerZ { get; }
        public ICollisionShape CollisionShape { get; }

        public CollisionBody(
            string entityInstanceID,
            string roomSpatialID,
            Vector2 position,
            int layerZ,
            ICollisionShape collisionShape)
        {
            EntityInstanceID = entityInstanceID;
            RoomSpatialID = roomSpatialID;

            Position = position;
            LayerZ = layerZ;

            CollisionShape = collisionShape;
        }
    }

    public class CollisionContext
    {
        // Final state
        public bool IsBlocked { get; set; }

        // Per-axis
        public bool BlockX { get; set; }
        public bool BlockY { get; set; }

        // Resolved layer
        public int LayerZ { get; set; }

        // Dynamic collisions
        public List<EntityInstance> Entities { get; }

        // Trigger entities
        public HashSet<string> Triggers { get; }

        public CollisionContext()
        {
            Entities = new List<EntityInstance>();
            Triggers = new HashSet<string>();
        }
    }

    public readonly struct ChunkKey : IEquatable<ChunkKey>
    {
        public readonly int X;
        public readonly int Y;

        public ChunkKey(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(ChunkKey other)
            => X == other.X && Y == other.Y;
    }

    public class CollisionService
    {
        #region Attributes
        private readonly HashSet<ChunkKey> visitedChunks = new();

        private readonly IRoomCache roomCache;
        private readonly WorldContext worldContext;
        #endregion

        #region Properties
        #endregion

        public CollisionService(
            IRoomCache roomCache,
            WorldContext worldContext)
        {
            this.roomCache = roomCache;
            this.worldContext = worldContext;
        }

        #region Methods
        public bool QueryInteractions(
                CollisionBody self,
                RoomSpatial roomSpatial,
                Vector2 position,
                CollisionContext context,
                Span<(int x, int y)> buffer)
        {
            bool isBlocked = false;
            visitedChunks.Clear();
            int count = self.CollisionShape.ComputeCells(position, buffer);

            for (int i = 0; i < count; i++)
            {
                var (cellX, cellY) = buffer[i];
                var (cx, cy) = ChunkMath.ToChunkOnly(cellX, cellY, Constraint.CHUNK_SIZE);
                var key = new ChunkKey(cx, cy);

                if (visitedChunks.Add(key))
                {
                    var (_, entityIds) = worldContext.QuerySpatial(self.RoomSpatialID, cellX, cellY, self.LayerZ);

                    foreach (var entityId in entityIds)
                    {
                        var entity = worldContext.GetEntity<EntityInstance>(entityId);
                        if (entity == null || entity.ID == self.EntityInstanceID) continue;

                        if (self.CollisionShape.Intersects(position, entity.CollisionShape, entity.Position))
                        {
                            if (!context.Entities.Contains(entity))
                                context.Entities.Add(entity);

                            if (entity.CollisionShape.IsBlocking) isBlocked = true;
                            if (entity.CollisionShape.IsTrigger) context.Triggers.Add(entity.ID);
                        }
                    }
                }

                var cell = roomCache.GetTopCell(roomSpatial.DefinitionID, cellX, cellY);
                if (cell != null && cell.Type != CellType.Walkable) isBlocked = true;
            }
            return isBlocked;
        }

        public CollisionContext QueryMovement(
            CollisionBody self,
            Vector2 desiredPosition)
        {
            var result = new CollisionContext();

            var roomSpatial = worldContext.GetRoom(self.RoomSpatialID);
            if (roomSpatial == null) return result;

            Span<(int x, int y)> buffer =
                stackalloc (int, int)[256];

            result.BlockX = QueryInteractions(
                self,
                roomSpatial,
                new Vector2(
                    desiredPosition.X,
                    self.Position.Y),
                result,
                buffer);

            result.BlockY = QueryInteractions(
                self,
                roomSpatial,
                new Vector2(
                    self.Position.X,
                    desiredPosition.Y),
                result,
                buffer);

            result.IsBlocked = result.BlockX || result.BlockY;

            result.LayerZ = ResolveLayer(
                self,
                roomSpatial,
                desiredPosition);

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

            // Single pass check
            var isBlocked = QueryInteractions(self, roomSpatial, position, result, buffer);

            result.IsBlocked = isBlocked;
            return result;
        }

        public void ValidateSpawn(
            ICollisionShape shape,
            string roomSpatialId,
            Vector2 position,
            int layerZ,
            string? ignoreEntityId = null)
        {
            var roomSpatial = worldContext.GetRoom(roomSpatialId);
            if (roomSpatial == null)
                throw new InternalException(
                    ResponseCode.CollisionService_RoomSpatialNotFoundOnValidateSpawn,
                    $"Room spatial with room spatial ID: {roomSpatialId} not found on validate spawn");

            Span<(int x, int y)> buffer = stackalloc (int, int)[256];

            int count = shape.ComputeCells(position, buffer);

            for (int i = 0; i < count; i++)
            {
                var (cellX, cellY) = buffer[i];

                var (_, entityIds) = worldContext.QuerySpatial(
                    roomSpatialId,
                    cellX,
                    cellY,
                    layerZ);

                foreach (var entityId in entityIds)
                {
                    if (entityId == ignoreEntityId)
                        continue;

                    var entity = worldContext.GetEntity<EntityInstance>(entityId);
                    if (entity == null)
                        continue;

                    bool intersects = shape.Intersects(
                        position,
                        entity.CollisionShape,
                        entity.Position);

                    if (!intersects)
                        continue;

                    if (entity.CollisionShape.IsBlocking)
                        throw new InternalException(
                            ResponseCode.CollisionService_SpawnBlockedByEntity,
                            $"Spawn blocked by entity instance ID: {entity.ID}");
                }

                var cell = roomCache.GetTopCell(
                    roomSpatial.DefinitionID,
                    cellX,
                    cellY);

                if (cell != null && cell.Type != CellType.Walkable)
                    throw new InternalException(
                        ResponseCode.CollisionService_SpawnBlockedByTile,
                        $"Spawn blocked by tile at ({cellX}, {cellY})");
            }
        }

        public void ValidateSpawnOnNotExistedRoom(
            ICollisionShape shape,
            string roomDefinitionId,
            Vector2 position,
            int layerZ)
        {
            Span<(int x, int y)> buffer = stackalloc (int, int)[256];

            int count = shape.ComputeCells(position, buffer);

            for (int i = 0; i < count; i++)
            {
                var (cellX, cellY) = buffer[i];

                var cell = roomCache.GetTopCell(
                    roomDefinitionId,
                    cellX,
                    cellY);

                if (cell != null && cell.Type != CellType.Walkable)
                    throw new InternalException(
                        ResponseCode.CollisionService_SpawnBlockedByTile,
                        $"Spawn blocked by tile at ({cellX}, {cellY})");
            }
        }

        private int ResolveLayer(
            CollisionBody self,
            RoomSpatial roomSpatial,
            Vector2 desiredPosition)
        {
            int wx = (int)MathF.Floor(desiredPosition.X);
            int wy = (int)MathF.Floor(desiredPosition.Y);

            var cell = roomCache.GetTopCell(
                roomSpatial.DefinitionID,
                wx,
                wy);

            return cell?.Z ?? self.LayerZ;
        }
        #endregion
    }
}