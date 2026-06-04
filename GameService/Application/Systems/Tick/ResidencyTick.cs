using Application.Context;
using Application.Persistence;
using Application.Services.WorldService;
using Domain.DomainException;
using Domain.Shared;
using System.Collections.Concurrent;

namespace Application.Systems.Tick
{
    public enum RoomResidencyState
    {
        Cold,
        Warm,
        Hot
    }

    public class RoomNode
    {
        public string RoomSpatialID { get; set; } = string.Empty;
        public RoomResidencyState State { get; set; }
        public DateTime LastAccessUtc { get; set; }
    }

    public class ResidencyTick
    {
        #region Attributes
        private readonly ConcurrentDictionary<string, SemaphoreSlim> roomLocks = new();
        private readonly ConcurrentDictionary<string, RoomNode> nodes = new();

        private readonly TimeSpan warmTTL = TimeSpan.FromSeconds(30);
        private readonly WorldContext worldContext;
        private readonly SnapshotPersistence snapshotPersistence;

        private float evictionAccumulator = 0;
        #endregion

        #region Properties
        #endregion

        public ResidencyTick(
            WorldContext worldContext,
            SnapshotPersistence snapshotPersistence)
        {
            this.worldContext = worldContext;
            this.snapshotPersistence = snapshotPersistence;
        }

        #region Methods
        public void TouchRoom(
            string roomId)
        {
            // Get or create room residency node
            var node = GetOrCreate(roomId);

            // Refresh node lifetime
            node.LastAccessUtc = DateTime.UtcNow;
        }

        public void MarkRoomHot(
            string roomId)
        {
            // Get or create room residency node
            var node = GetOrCreate(roomId);

            // Mark as active/hot
            node.State = RoomResidencyState.Hot;
            node.LastAccessUtc = DateTime.UtcNow;
        }

        public void MarkRoomExited(
            string roomId)
        {
            // Get or create room residency node
            var node = GetOrCreate(roomId);

            // Downgrade active room to warm state
            if (node.State == RoomResidencyState.Hot)
            {
                node.State = RoomResidencyState.Warm;
            }

            // Refresh node lifetime
            node.LastAccessUtc = DateTime.UtcNow;
        }

        public async Task<RoomSnapshot> EnsureRoomLoaded(
            string roomSpatialId)
        {
            var roomLock = GetRoomLock(roomSpatialId);
            await roomLock.WaitAsync();

            RoomNode node;
            RoomSnapshot? snapshot;
            bool needLoadFromPersistence = false;

            try
            {
                // Get or create node
                node = GetOrCreate(roomSpatialId);

                // Already loaded in runtime (Warm/Hot)
                if (node.State != RoomResidencyState.Cold)
                {
                    node.LastAccessUtc = DateTime.UtcNow;

                    var room = worldContext.GetRoom(roomSpatialId);
                    if (room == null)
                        throw new InternalException(
                            ResponseCode.ResidencyTick_RoomSpatialNotFoundInRuntime,
                            $"Room spatial not found when ensured from loaded: {roomSpatialId}");

                    return new RoomSnapshot
                    {
                        Room = room,
                        Entities = worldContext
                            .GetEntitiesByRoom(roomSpatialId)
                            .ToList()
                    };
                }

                // Mark intention to load (still inside lock)
                needLoadFromPersistence = true;

                snapshot = await snapshotPersistence.LoadRoomSnapshotAsync(roomSpatialId);
                if (snapshot == null)
                    throw new InternalException(
                        ResponseCode.ResidencyTick_RoomSnapshotNotFoundInPersistence,
                        $"Room snapshot not found in persistence when ensure loaded: {roomSpatialId}");

                // Update state only (no world mutation yet)
                node.State = RoomResidencyState.Warm;
                node.LastAccessUtc = DateTime.UtcNow;
            }
            finally
            {
                roomLock.Release();
            }

            // IMPORTANT: mutate runtime OUTSIDE lock
            if (needLoadFromPersistence && snapshot != null)
            {
                worldContext.Load(snapshot);
            }

            return snapshot!;
        }

        public async Task Tick(
            float dt)
        {
            evictionAccumulator += dt;

            if (evictionAccumulator < 1f)
                return; // only run every 1 second

            evictionAccumulator = 0;

            var now = DateTime.UtcNow;

            foreach (var node in nodes.Values)
            {
                if (node.State == RoomResidencyState.Hot)
                    continue;

                // No player inside room then unloaded room soon
                if (node.State == RoomResidencyState.Warm)
                {
                    if (now - node.LastAccessUtc > warmTTL)
                    {
                        await EvictToCold(node);
                    }
                }
            }
        }

        private async Task EvictToCold(RoomNode node)
        {
            var roomLock = GetRoomLock(node.RoomSpatialID);
            await roomLock.WaitAsync();

            RoomSnapshot? snapshot;
            bool shouldEvict = false;

            try
            {
                // Already cold
                if (node.State == RoomResidencyState.Cold)
                    return;

                shouldEvict = true;

                // Only mark state change inside lock
                node.State = RoomResidencyState.Cold;
                node.LastAccessUtc = DateTime.UtcNow;

                // IMPORTANT: unload runtime state inside lock is OK ONLY if it's fast & deterministic
                snapshot = worldContext.Unload(node.RoomSpatialID);
            }
            finally
            {
                roomLock.Release();
            }

            // Do persistence OUTSIDE lock
            if (shouldEvict && snapshot != null)
            {
                await snapshotPersistence.SaveRoomSnapshotAsync(snapshot);
            }
        }

        private RoomNode GetOrCreate(
            string roomId)
        {
            if (!nodes.TryGetValue(roomId, out var node))
            {
                node = new RoomNode
                {
                    RoomSpatialID = roomId,
                    State = RoomResidencyState.Cold,
                    LastAccessUtc = DateTime.UtcNow
                };

                nodes[roomId] = node;
            }

            return node;
        }

        private SemaphoreSlim GetRoomLock(string roomId)
        {
            return roomLocks.GetOrAdd(
                roomId,
                _ => new SemaphoreSlim(1, 1));
        }
        #endregion
    }
}