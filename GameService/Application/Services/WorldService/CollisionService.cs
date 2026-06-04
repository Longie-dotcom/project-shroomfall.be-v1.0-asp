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
        public CollisionContext QueryMovement(
            CollisionBody self,
            Vector2 desiredPosition)
        {
            var result = new CollisionContext();

            var roomSpatial = worldContext.GetRoom(self.RoomSpatialID);
            if (roomSpatial == null)
                throw new InternalException(
                    ResponseCode.CollisionService_RoomSpatialNotFoundOnQueryMovement,
                    $"Room spatial with room spatial ID: {self.RoomSpatialID} not found on query movement" +
                    $", referenced from entity instance: {self.EntityInstanceID}");

            var currentPos = self.Position;

            // axis separated movement
            var testPosX = new Vector2(
                desiredPosition.X,
                currentPos.Y);

            var testPosY = new Vector2(
                currentPos.X,
                desiredPosition.Y);

            Span<(int x, int y)> buffer =
                stackalloc (int, int)[256];

            bool blockX = ProcessAxis(
                self,
                roomSpatial,
                testPosX,
                result,
                buffer);

            bool blockY = ProcessAxis(
                self,
                roomSpatial,
                testPosY,
                result,
                buffer);

            result.BlockX = blockX;
            result.BlockY = blockY;
            result.IsBlocked = blockX || blockY;

            result.LayerZ = ResolveLayer(
                self,
                roomSpatial,
                desiredPosition);

            return result;
        }

        public void ValidateSpawn(
            ICollisionShape shape,
            string roomSpatialId,
            Vector2 position,
            int layerZ)
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

        private bool ProcessAxis(
            CollisionBody self,
            RoomSpatial roomSpatial,
            Vector2 testPosition,
            CollisionContext result,
            Span<(int x, int y)> buffer)
        {
            bool blocked = false;

            visitedChunks.Clear();

            int count = self.CollisionShape.ComputeCells(
                testPosition,
                buffer);

            for (int i = 0; i < count; i++)
            {
                var (cellX, cellY) = buffer[i];

                var (cx, cy) = ChunkMath.ToChunkOnly(
                    cellX,
                    cellY,
                    Constraint.CHUNK_SIZE);

                var key = new ChunkKey(cx, cy);

                // already checked this chunk
                bool queriedChunk = visitedChunks.Add(key);

                if (queriedChunk)
                {
                    // =====================================================
                    // DYNAMIC ENTITIES
                    // =====================================================

                    var (_, entityIds) = worldContext.QuerySpatial(
                        self.RoomSpatialID,
                        cellX,
                        cellY,
                        self.LayerZ);

                    foreach (var entityId in entityIds)
                    {
                        var entity = worldContext.GetEntity<EntityInstance>(entityId);
                        if (entity == null)
                            continue;

                        if (entity.ID == self.EntityInstanceID)
                            continue;

                        bool intersects =
                            self.CollisionShape.Intersects(
                                testPosition,
                                entity.CollisionShape,
                                entity.Position);

                        if (!intersects)
                            continue;

                        if (!result.Entities.Contains(entity))
                            result.Entities.Add(entity);

                        if (entity.CollisionShape.IsBlocking)
                            blocked = true;

                        if (entity.CollisionShape.IsTrigger)
                            result.Triggers.Add(entity.ID);
                    }
                }

                // =====================================================
                // STATIC CELL
                // =====================================================
                var cell = roomCache.GetTopCell(
                    roomSpatial.DefinitionID,
                    cellX,
                    cellY);

                if (cell == null)
                    continue;

                if (cell.Type != CellType.Walkable)
                    blocked = true;
            }

            return blocked;
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