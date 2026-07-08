using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Admin;
using Application.Interfaces.Utility;
using Application.Persistence;
using Contract;
using Domain.DomainException;
using ResponseCode;
using System.Collections.Concurrent;

namespace Application.Services.WorldService
{
    internal enum RoomResidencyState
    {
        Cold,
        Warm,
        Hot
    }

    internal enum RoomResidencyPolicy
    {
        Dynamic,
        Permanent
    }

    internal class RoomNode
    {
        public string RoomSpatialID { get; set; } = string.Empty;
        public RoomResidencyState State { get; set; }
        public RoomResidencyPolicy Policy { get; set; }
        public DateTime LastAccessUtc { get; set; }
        public HashSet<string> ActivePlayerInstanceIds { get; } = new();
    }

    public class ResidencyService
    {
        #region Attributes
        private readonly ITelemetryQueue telemetryQueue;
        private readonly IEventBus eventBus;
        private readonly WorldContext worldContext;
        private readonly SnapshotPersistence snapshotPersistence;

        private readonly ConcurrentDictionary<string, SemaphoreSlim> roomLocks = new();
        private readonly ConcurrentDictionary<string, RoomNode> nodes = new();

        private readonly TimeSpan warmTTL = TimeSpan.FromSeconds(30);
        private float evictionAccumulator = 0;
        private float telemetryAccumulator = 0;
        #endregion

        #region Properties
        #endregion

        public ResidencyService(
            ITelemetryQueue telemetryQueue,
            IEventBus eventBus,
            WorldContext worldContext,
            SnapshotPersistence snapshotPersistence)
        {
            this.telemetryQueue = telemetryQueue;
            this.eventBus = eventBus;
            this.worldContext = worldContext;
            this.snapshotPersistence = snapshotPersistence;
        }

        #region Methods
        public async Task JoinRoomAsync(
            string roomId, 
            string playerInstanceId)
        {
            var roomLock = GetRoomLock(roomId);
            await roomLock.WaitAsync();
            try
            {
                var node = GetOrCreate(roomId);
                var oldState = node.State;

                // Thread-safely register player presence
                node.ActivePlayerInstanceIds.Add(playerInstanceId);
                node.LastAccessUtc = DateTime.UtcNow;

                // Automate State Escalation: Elevate room state to Hot if it wasn't already
                if (node.State != RoomResidencyState.Hot)
                {
                    node.State = RoomResidencyState.Hot;

                    eventBus.Publish(new RoomResidencyChangedEvent(
                        node.RoomSpatialID,
                        oldState.ToString(),
                        RoomResidencyState.Hot.ToString()));
                }
            }
            finally
            {
                roomLock.Release();
            }
        }

        public async Task LeaveRoomAsync(
            string roomId, 
            string playerInstanceId)
        {
            var roomLock = GetRoomLock(roomId);
            await roomLock.WaitAsync();
            try
            {
                if (!nodes.TryGetValue(roomId, out var node))
                    return;

                // Thread-safely remove player presence
                node.ActivePlayerInstanceIds.Remove(playerInstanceId);
                node.LastAccessUtc = DateTime.UtcNow;

                // Automate State Downgrade: If the room became completely empty, drop it to Warm
                if (node.ActivePlayerInstanceIds.Count == 0 && node.State == RoomResidencyState.Hot)
                {
                    if (node.Policy == RoomResidencyPolicy.Permanent)
                        return; // Permanent rooms ignore automatic dynamic downgrades

                    node.State = RoomResidencyState.Warm;

                    eventBus.Publish(new RoomResidencyChangedEvent(
                        node.RoomSpatialID,
                        RoomResidencyState.Hot.ToString(),
                        RoomResidencyState.Warm.ToString()));
                }
            }
            finally
            {
                roomLock.Release();
            }
        }

        public async Task<RoomSnapshot> EnsureRoomLoaded(
            string roomSpatialId)
        {
            var roomLock = GetRoomLock(roomSpatialId);
            await roomLock.WaitAsync();

            RoomNode node;
            RoomSnapshot? snapshot;
            var oldState = RoomResidencyState.Cold;

            try
            {
                node = GetOrCreate(roomSpatialId);
                oldState = node.State;

                snapshot = TryGetRuntimeSnapshot(roomSpatialId);
                if (snapshot != null)
                    return snapshot;

                snapshot = await snapshotPersistence.LoadRoomSnapshotAsync(roomSpatialId);
                if (snapshot == null)
                    throw new InternalException(
                        ApplicationCode.ResidencyServiceCode.RoomSnapshotNotFoundInPersistence,
                        $"Residency synchronization failed. Cold state Room '{roomSpatialId}' contains no archived record state inside snapshot persistence layer.");

                worldContext.Load(snapshot);

                node.State = RoomResidencyState.Warm;
                node.LastAccessUtc = DateTime.UtcNow;
            }
            finally
            {
                roomLock.Release();
            }

            eventBus.Publish(new RoomResidencyChangedEvent(
                roomSpatialId,
                oldState.ToString(),
                RoomResidencyState.Warm.ToString()));

            return snapshot;
        }

        public void MarkRoomPermanent(
            string roomId)
        {
            var node = GetOrCreate(roomId);

            var oldState = node.State;

            node.Policy = RoomResidencyPolicy.Permanent;
            node.State = RoomResidencyState.Hot;
            node.LastAccessUtc = DateTime.UtcNow;

            if (oldState != RoomResidencyState.Hot)
            {
                eventBus.Publish(new RoomResidencyChangedEvent(
                    node.RoomSpatialID,
                    oldState.ToString(),
                    RoomResidencyState.Hot.ToString()));
            }
        }

        public RoomSnapshot RegisterRuntimeRoom(
            RoomSnapshot snapshot)
        {
            var roomLock = GetRoomLock(snapshot.Room.ID);
            roomLock.Wait();

            try
            {
                var node = GetOrCreate(snapshot.Room.ID);

                if (node.State != RoomResidencyState.Cold)
                    return TryGetRuntimeSnapshot(snapshot.Room.ID)!;

                worldContext.Load(snapshot);

                node.State = RoomResidencyState.Warm;
                node.LastAccessUtc = DateTime.UtcNow;
            }
            finally
            {
                roomLock.Release();
            }

            eventBus.Publish(new RoomResidencyChangedEvent(
                snapshot.Room.ID,
                RoomResidencyState.Cold.ToString(),
                RoomResidencyState.Warm.ToString()));

            return snapshot;
        }

        public async Task Tick(
            float dt)
        {
            evictionAccumulator += dt;
            telemetryAccumulator += dt;

            // 1. Core State Processing Loop
            if (evictionAccumulator >= Constraint.RESIDENCY_TICK_PER_SECOND)
            {
                evictionAccumulator = 0;
                var now = DateTime.UtcNow;

                foreach (var node in nodes.Values)
                {
                    if (node.Policy == RoomResidencyPolicy.Permanent)
                        continue;

                    if (node.State == RoomResidencyState.Hot)
                        continue;

                    if (node.State == RoomResidencyState.Warm && now - node.LastAccessUtc > warmTTL)
                    {
                        await EvictToCold(node);
                    }
                }
            }

            // 2. Metrics Accumulation & Aggregation Telemetry
            if (telemetryAccumulator >= Constraint.RESIDENCY_REPORT_PER_SECOND)
            {
                telemetryAccumulator = 0;

                int coldCount = 0;
                int warmCount = 0;
                int hotCount = 0;

                foreach (var node in nodes.Values)
                {
                    switch (node.State)
                    {
                        case RoomResidencyState.Cold: coldCount++; break;
                        case RoomResidencyState.Warm: warmCount++; break;
                        case RoomResidencyState.Hot: hotCount++; break;
                    }
                }

                // Append telemetry snapshot data to tracking stream safely
                telemetryQueue.EnqueueAlert(
                    ApplicationCode.ResidencyServiceCode.StateHeartbeatReport,
                    $"Residency performance update. Active clusters tracked -> Hot: {hotCount} | Warm: {warmCount} | Cold: {coldCount} (Total Tracked Rooms: {nodes.Count})",
                    TelemetrySeverity.Info);
            }
        }

        private RoomSnapshot? TryGetRuntimeSnapshot(
            string roomSpatialId)
        {
            var node = GetOrCreate(roomSpatialId);

            if (node.State == RoomResidencyState.Cold)
                return null;

            node.LastAccessUtc = DateTime.UtcNow;

            var room = worldContext.GetRoom(roomSpatialId);
            if (room == null)
                throw new InternalException(
                    ApplicationCode.ResidencyServiceCode.RoomSpatialNotFoundInRuntime,
                    $"Residency synchronization failed. Room '{roomSpatialId}' is state marked '{node.State}' but could not be located inside runtime memory context.");

            return new RoomSnapshot
            {
                Room = room,
                Entities = worldContext
                    .GetEntitiesByRoom(roomSpatialId)
                    .ToList()
            };
        }

        private async Task EvictToCold(
            RoomNode node)
        {
            var roomLock = GetRoomLock(node.RoomSpatialID);
            await roomLock.WaitAsync();

            RoomSnapshot? snapshot;
            bool shouldEvict = false;
            var oldState = node.State;

            try
            {
                if (node.State == RoomResidencyState.Cold)
                {
                    roomLock.Release();
                    return;
                }

                shouldEvict = true;
                node.State = RoomResidencyState.Cold;
                node.LastAccessUtc = DateTime.UtcNow;

                // Instantly clear memory while on the main thread loop
                snapshot = worldContext.Unload(node.RoomSpatialID);
            }
            catch
            {
                roomLock.Release();
                throw;
            }

            if (shouldEvict)
            {
                eventBus.Publish(new RoomResidencyChangedEvent(
                    node.RoomSpatialID,
                    oldState.ToString(),
                    RoomResidencyState.Cold.ToString()));
            }

            // Fire-and-forget or offload to background thread pool.
            if (shouldEvict && snapshot != null)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        // Background thread does the slow work while holding the lock
                        await snapshotPersistence.SaveRoomSnapshotAsync(snapshot);
                    }
                    catch (Exception ex)
                    {
                        telemetryQueue.EnqueueAlert(
                            ApplicationCode.ResidencyServiceCode.RoomSnapshotPersistenceFailed,
                            $"Background eviction snapshot save failed for room '{node.RoomSpatialID}'. Exception: {ex.Message}",
                            TelemetrySeverity.Error);
                    }
                    finally
                    {
                        // ONLY release the lock once the database safely has the data
                        roomLock.Release();
                    }
                });
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