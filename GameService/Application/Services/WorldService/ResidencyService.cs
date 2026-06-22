using Application.Context;
using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Admin;
using Application.Interfaces.Utility;
using Application.Persistence;
using Contract;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;
using System.Collections.Concurrent;

namespace Application.Services.WorldService
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
            var oldState = node.State;

            // Mark as active/hot
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

        public void MarkRoomExited(
            string roomId)
        {
            // Get or create room residency node
            var node = GetOrCreate(roomId);
            var oldState = node.State;

            // Downgrade active room to warm state
            if (node.State == RoomResidencyState.Hot)
            {
                node.State = RoomResidencyState.Warm;
                node.LastAccessUtc = DateTime.UtcNow;

                // Dispatch Real-time Event
                eventBus.Publish(new RoomResidencyChangedEvent(
                    node.RoomSpatialID,
                    oldState.ToString(),
                    RoomResidencyState.Warm.ToString()));
            }
            else
            {
                // Refresh node life time
                node.LastAccessUtc = DateTime.UtcNow;
            }
        }

        public async Task<RoomSnapshot> EnsureRoomLoaded(
            string roomSpatialId)
        {
            var roomLock = GetRoomLock(roomSpatialId);
            await roomLock.WaitAsync();

            RoomNode node;
            RoomSnapshot? snapshot;
            bool needLoadFromPersistence = false;
            var oldState = RoomResidencyState.Cold;

            try
            {
                // Get or create node
                node = GetOrCreate(roomSpatialId);
                oldState = node.State;

                // Already loaded in runtime (Warm/Hot)
                if (node.State != RoomResidencyState.Cold)
                {
                    node.LastAccessUtc = DateTime.UtcNow;

                    var room = worldContext.GetRoom(roomSpatialId);
                    if (room == null)
                        throw new InternalException(
                            ApplicationCode.ResidencyTickCode.RoomSpatialNotFoundInRuntime,
                            $"Residency synchronization failed. Room '{roomSpatialId}' is state marked '{node.State}' but could not be located inside runtime memory context.");

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
                        ApplicationCode.ResidencyTickCode.RoomSnapshotNotFoundInPersistence,
                        $"Residency synchronization failed. Cold state Room '{roomSpatialId}' contains no archived record state inside snapshot persistence layer.");

                // Update state only (no world mutation yet)
                node.State = RoomResidencyState.Warm;
                node.LastAccessUtc = DateTime.UtcNow;
            }
            finally
            {
                roomLock.Release();
            }

            if (needLoadFromPersistence && snapshot != null)
            {
                worldContext.Load(snapshot);

                eventBus.Publish(new RoomResidencyChangedEvent(
                    roomSpatialId,
                    oldState.ToString(),
                    RoomResidencyState.Warm.ToString()));
            }

            return snapshot!;
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
                    ApplicationCode.ResidencyTickCode.StateHeartbeatReport,
                    $"Residency performance update. Active clusters tracked -> Hot: {hotCount} | Warm: {warmCount} | Cold: {coldCount} (Total Tracked Rooms: {nodes.Count})",
                    TelemetrySeverity.Info);
            }
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
                    return;

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
                            ApplicationCode.ResidencyTickCode.RoomSnapshotPersistenceFailed,
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