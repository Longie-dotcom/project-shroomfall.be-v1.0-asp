using Application.Context;
using Application.Events.Event;
using Application.Interfaces.Factory;
using Application.Interfaces.Realtime;
using Application.Persistence;
using Application.Services.WorldService;
using Application.Systems.Tick;
using Contract.Enum.WorldDomain;
using Domain.Common;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Shared;

namespace Application.Coordinator
{
    public class PlayerCoordinator
    {
        #region Attributes
        private readonly IEventBus eventBus;
        private readonly IPlayerInstanceFactory playerInstanceFactory;
        private readonly SnapshotPersistence snapshotPersistence;
        private readonly RoomConnectionPersistence roomConnectionPersistence;
        private readonly EntityPersistence entityPersistence;
        private readonly ResidencyTick residencyTick;
        private readonly WorldContext worldContext;
        private readonly PlayerContext playerContext;
        private readonly SpawnService spawnService;
        private readonly CollisionService collisionService;
        private readonly InitializationService initializationService;
        private readonly TopologyService topologyService;
        #endregion

        #region Properties
        #endregion

        public PlayerCoordinator(
            IEventBus eventBus,
            IPlayerInstanceFactory playerInstanceFactory,
            SnapshotPersistence snapshotPersistence,
            RoomConnectionPersistence roomConnectionPersistence,
            EntityPersistence entityPersistence,
            ResidencyTick residencyTick,
            WorldContext worldContext,
            PlayerContext playerContext,
            SpawnService spawnService,
            CollisionService collisionService,
            InitializationService initializationService,
            TopologyService topologyService)
        {
            this.eventBus = eventBus;
            this.playerInstanceFactory = playerInstanceFactory;
            this.snapshotPersistence = snapshotPersistence;
            this.roomConnectionPersistence = roomConnectionPersistence;
            this.entityPersistence = entityPersistence;
            this.residencyTick = residencyTick;
            this.worldContext = worldContext;
            this.playerContext = playerContext;
            this.spawnService = spawnService;
            this.collisionService = collisionService;
            this.initializationService = initializationService;
            this.topologyService = topologyService;
        }

        #region Methods
        public async Task<PlayerInstance> CreatePlayer(
            string playerDefinitionId,
            string roomDefinitionId,
            string userId)
        {
            // Generate IDs
            var roomSpatialId = $"PLAYER_ROOM_{userId}_{Guid.NewGuid():N}";
            var playerInstanceId = $"PLAYER_{userId}_{Guid.NewGuid():N}";

            // Create room snapshot
            var roomSnapshot = initializationService.InitializeRoom(
                roomDefinitionId: roomDefinitionId,
                roomSpatialId: roomSpatialId,
                ownerId: playerInstanceId);

            // Create player instance
            var player = CreatePlayerInstance(
                playerDefinitionId,
                playerInstanceId,
                userId,
                roomSnapshot);

            // Mutate snapshot
            roomSnapshot.Entities.Add(player);

            // Persist snapshot
            await snapshotPersistence.SaveRoomSnapshotAsync(roomSnapshot);

            return player;
        }

        public async Task<(PlayerInstance, RoomSnapshot)> LoadPlayer(
            string playerInstanceId,
            string userId)
        {
            // Load persisted player
            var player = await RequirePersistedPlayer(playerInstanceId);

            // Validate ownership
            ValidatePlayerOwnership(player, userId);

            // Ensure room residency
            var snapshot = await residencyTick.EnsureRoomLoaded(player.RoomSpatialID);

            // Validate spawn
            collisionService.ValidateSpawn(
                shape: player.CollisionShape,
                roomSpatialId: player.RoomSpatialID,
                position: player.Position,
                layerZ: player.LayerZ);

            // Mutate runtime
            LoadPlayerRuntime(player);

            // Publish events
            PublishPlayerLoaded(player);

            return (player, snapshot);
        }

        public async Task UnloadPlayer(
            string playerInstanceId)
        {
            // Get runtime player
            var player = RequireRuntimePlayer(playerInstanceId);

            // Persist player
            await entityPersistence.SaveAsync(player);

            // Mutate runtime
            UnloadPlayerRuntime(player);

            // Publish events
            PublishPlayerUnloaded(player);
        }

        public async Task<RoomSnapshot> PlayerTouchEntity(
            string playerInstanceId,
            string targetEntityInstanceId)
        {
            // Validate runtime player
            RequireRuntimePlayer(playerInstanceId);

            // Resolve topology
            var (connection, roomSnapshot, isNew) = await topologyService.ResolveOrCreateConnection(targetEntityInstanceId);

            // Save the target spatial room ID
            var toRoomId = connection.DestinationRoomSpatialID!;

            // Persist if room is first touched
            if (isNew && roomSnapshot != null)
            {
                await snapshotPersistence.SaveRoomSnapshotAsync(roomSnapshot!);
                await roomConnectionPersistence.SaveAsync(connection);

                // Runtime mutation
                worldContext.AddConnection(connection);
            }

            // Change room
            return await PlayerChangeRoom(playerInstanceId, toRoomId);
        }

        private async Task<RoomSnapshot> PlayerChangeRoom(
            string playerInstanceId,
            string targetRoomId)
        {
            // Get runtime player
            var player = RequireRuntimePlayer(playerInstanceId);

            // Ensure room residency
            var snapshot = await residencyTick.EnsureRoomLoaded(targetRoomId);

            // Resolve spawn
            var (position, layerZ) = ResolveValidSpawn(player, snapshot);

            var fromRoomId = player.RoomSpatialID;

            // Mutate runtime
            worldContext.ChangeRoom(playerInstanceId, position, layerZ, targetRoomId);

            // Mutate membership
            ApplyMembershipTransition(player, fromRoomId, targetRoomId);

            // Mutate residency
            ApplyResidencyTransition(fromRoomId, targetRoomId);

            // Publish events
            PublishRoomTransition(player, fromRoomId, targetRoomId);

            return snapshot;
        }

        private PlayerInstance RequireRuntimePlayer(
            string playerInstanceId)
        {
            var player = worldContext.GetEntity<PlayerInstance>(playerInstanceId);
            if (player == null)
                throw new InternalException(
                    ResponseCode.PlayerCoordinator_PlayerInstanceNotFoundInRuntime,
                    $"Player instance in runtime with instance ID: {playerInstanceId} is not found");

            return player;
        }

        private async Task<PlayerInstance> RequirePersistedPlayer(
            string playerInstanceId)
        {
            var player = await entityPersistence.LoadAsync<PlayerInstance>(playerInstanceId);
            if (player == null)
                throw new InternalException(
                    ResponseCode.PlayerCoordinator_PlayerInstanceNotFoundInPersistence,
                    $"Player instance on load with instance ID: {playerInstanceId} is not found");

            return player;
        }

        private PlayerInstance CreatePlayerInstance(
            string playerDefinitionId,
            string playerInstanceId,
            string userId,
            RoomSnapshot roomSnapshot)
        {
            var (position, layerZ) = spawnService.ResolveSpawnPosition(
                roomSnapshot.Room.DefinitionID,
                playerDefinitionId,
                SpawnRuleType.Player);

            var player = playerInstanceFactory.Create(
                definitionId: playerDefinitionId,
                instanceId: playerInstanceId,
                userId: userId,
                roomSpatialId: roomSnapshot.Room.ID,
                layerZ: layerZ,
                position: position,
                direction: new Vector2(0, 1));

            collisionService.ValidateSpawn(
                shape: player.CollisionShape,
                roomSpatialId: roomSnapshot.Room.ID,
                position: position,
                layerZ: layerZ);

            return player;
        }

        private void ValidatePlayerOwnership(
            PlayerInstance player, 
            string userId)
        {
            if (userId != player.UserID)
                throw new Unauthorized(
                    ResponseCode.PlayerCoordinator_UnauthorizedPlayerInstance,
                    $"Player session is unauthorized");
        }

        private void LoadPlayerRuntime(
            PlayerInstance player)
        {
            worldContext.AddEntity(player);

            playerContext.JoinRoom(
                player.RoomSpatialID,
                player.ID);

            residencyTick.MarkRoomHot(player.RoomSpatialID);
        }

        private void UnloadPlayerRuntime(
            PlayerInstance player)
        {
            worldContext.RemoveEntity(player.ID);

            playerContext.LeaveRoom(
                player.RoomSpatialID,
                player.ID);

            if (playerContext.IsRoomEmpty(player.RoomSpatialID))
                residencyTick.MarkRoomExited(player.RoomSpatialID);
        }

        private (Vector2 Position, int LayerZ) ResolveValidSpawn(
            PlayerInstance player, 
            RoomSnapshot snapshot)
        {
            var (position, layerZ) = spawnService.ResolveSpawnPosition(
                snapshot.Room.DefinitionID,
                player.DefinitionID,
                SpawnRuleType.Player);

            collisionService.ValidateSpawn(
                shape: player.CollisionShape,
                roomSpatialId: snapshot.Room.ID,
                position: position,
                layerZ: layerZ);

            return (position, layerZ);
        }

        private void ApplyMembershipTransition(
            PlayerInstance player, 
            string fromRoomId, 
            string toRoomId)
        {
            playerContext.LeaveRoom(fromRoomId, player.ID);
            playerContext.JoinRoom(toRoomId, player.ID);
        }

        private void ApplyResidencyTransition(
            string fromRoomId, 
            string toRoomId)
        {
            residencyTick.MarkRoomHot(toRoomId);

            if (playerContext.IsRoomEmpty(fromRoomId))
                residencyTick.MarkRoomExited(fromRoomId);
            else
                residencyTick.TouchRoom(fromRoomId);
        }

        private void PublishPlayerLoaded(
            PlayerInstance player)
        {
            eventBus.Publish(new EntityLifecycleEvent(
                player,
                player.RoomSpatialID,
                EntityLifecycleType.Spawn));

            eventBus.Publish(new PlayerGroupedEvent(
                player.UserID,
                null,
                player.RoomSpatialID));
        }

        private void PublishPlayerUnloaded(
            PlayerInstance player)
        {
            eventBus.Publish(new EntityLifecycleEvent(
                player,
                player.RoomSpatialID,
                EntityLifecycleType.Despawn));

            eventBus.Publish(new PlayerGroupedEvent(
                player.UserID,
                player.RoomSpatialID,
                null));
        }

        private void PublishRoomTransition(
            PlayerInstance player, 
            string fromRoomId, 
            string toRoomId)
        {
            eventBus.Publish(new EntityLifecycleEvent(
                player,
                fromRoomId,
                EntityLifecycleType.Despawn));

            eventBus.Publish(new EntityLifecycleEvent(
                player,
                toRoomId,
                EntityLifecycleType.Spawn));

            eventBus.Publish(new PlayerGroupedEvent(
                player.UserID,
                fromRoomId,
                toRoomId));
        }
        #endregion
    }
}