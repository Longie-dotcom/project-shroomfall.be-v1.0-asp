using Application.Context;
using Application.Persistence;
using Application.Services.WorldService;
using Domain.DomainException;
using Domain.Runtime.WorldDomain;
using Domain.Shared;

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
        private readonly Dictionary<string, RoomNode> nodes = new();
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
            // Get or create room residency node
            var node = GetOrCreate(roomSpatialId);

            // Already resident
            if (node.State != RoomResidencyState.Cold)
            {
                // Refresh node lifetime
                node.LastAccessUtc = DateTime.UtcNow;

                // Retrieve room on runtime
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

            // Retrieve from persistence
            var snapshot = await snapshotPersistence.LoadRoomSnapshotAsync(roomSpatialId);
            if (snapshot == null)
                throw new InternalException(
                    ResponseCode.ResidencyTick_RoomSnapshotNotFoundInPersistence,
                    $"Room snapshot not found in persistence when ensure loaded: {roomSpatialId}");

            // Reload room graph to runtime
            worldContext.Load(
                new WorldGraph
                {
                    Rooms = new List<RoomSpatial> { snapshot.Room },
                    Entities = snapshot.Entities
                });

            // Refresh node lifetime
            node.State = RoomResidencyState.Warm;
            node.LastAccessUtc = DateTime.UtcNow;

            return snapshot;
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

        private async Task EvictToCold(
            RoomNode node)
        {
            // Already cold
            if (node.State == RoomResidencyState.Cold)
                return;

            // Snapshot runtime state
            // (NOTE: ALREADY ENSURED ENVIRONMENT ENTITIES UNLOADING ONLY)
            var snapshot = worldContext.Unload(node.RoomSpatialID);

            if (snapshot != null)
            {
                await snapshotPersistence.SaveRoomSnapshotAsync(snapshot);
            }

            // Mark as cold
            node.State = RoomResidencyState.Cold;
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
        #endregion
    }
}