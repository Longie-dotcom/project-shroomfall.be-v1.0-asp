using Application.Interfaces.Cache;
using Application.Services.Abstraction.WorldService;
using Domain.Abstraction;
using Domain.Abstraction.World;
using Domain.Common;
using Domain.Definition.WorldDomain.Enum;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.WorldDomain.World;
using Domain.Shared;

namespace Application.Services.WorldService
{
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

        public override bool Equals(object? obj)
            => obj is ChunkKey other && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(X, Y);
    }

    public class CollisionService : ICollisionService
    {
        #region Attributes
        private readonly HashSet<ChunkKey> visitedChunks = new();

        private readonly IRoomCache roomCache;
        private readonly IWorldQuery world;
        #endregion

        #region Properties
        #endregion

        public CollisionService(
            IRoomCache roomCache,
            IWorldQuery world)
        {
            this.roomCache = roomCache;
            this.world = world;
        }

        #region Methods
        public CollisionContext QueryMovement(
            CollisionBody self,
            Vector2 desiredPosition)
        {
            var result = new CollisionContext();

            var roomSpatial = world.GetRoom(self.RoomSpatialID);

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

        public CollisionContext QueryPoint(
            ICollisionShape shape,
            string roomSpatialId,
            Vector2 position,
            int layerZ)
        {
            var result = new CollisionContext();

            var roomSpatial = world.GetRoom(roomSpatialId);

            Span<(int x, int y)> buffer = stackalloc (int, int)[256];

            int count = shape.ComputeCells(position, buffer);

            for (int i = 0; i < count; i++)
            {
                var (cellX, cellY) = buffer[i];

                var (_, entityIds) = world.QuerySpatial(
                    roomSpatialId,
                    cellX,
                    cellY,
                    layerZ);

                foreach (var entityId in entityIds)
                {
                    var entity = world.Get<EntityInstance>(entityId);
                    if (entity == null)
                        continue;

                    bool intersects =
                        shape.Intersects(position, entity.CollisionShape, entity.Position);

                    if (!intersects)
                        continue;

                    result.Entities.Add(entity);

                    if (entity.CollisionShape.IsBlocking)
                        result.IsBlocked = true;

                    if (entity.CollisionShape.IsTrigger)
                        result.Triggers.Add(entity.ID);
                }

                var cell = roomCache.GetTopCell(
                    roomSpatial.DefinitionID,
                    cellX,
                    cellY);

                if (cell != null && cell.Tile.Type != TileType.Walkable)
                    result.IsBlocked = true;
            }

            return result;
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

                    var (_, entityIds) = world.QuerySpatial(
                        self.RoomSpatialID,
                        cellX,
                        cellY,
                        self.LayerZ);

                    foreach (var entityId in entityIds)
                    {
                        var entity =
                            world.Get<EntityInstance>(entityId);

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

                if (cell.Tile.Type != TileType.Walkable)
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