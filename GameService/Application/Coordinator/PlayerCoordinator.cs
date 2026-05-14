using Application.Context;
using Application.Events.Event;
using Application.Interfaces.Realtime;
using Application.Persistence;
using Application.Services.WorldService;
using Application.Systems.Tick;
using Domain.Definition.WorldDomain.Enum;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Shared;

namespace Application.Coordinator
{
    public class PlayerCoordinator
    {
        #region Attributes
        private readonly IEventBus eventBus;
        private readonly SnapshotPersistence snapshotPersistence;
        private readonly EntityPersistence entityPersistence;
        private readonly ResidencyTick residencyTick;
        private readonly WorldContext worldContext;
        private readonly PlayerContext playerContext;
        private readonly CreationService creationService;
        private readonly SpawnService spawnService;
        private readonly CollisionService collisionService;
        #endregion

        #region Properties
        #endregion

        public PlayerCoordinator(
            IEventBus eventBus,
            SnapshotPersistence snapshotPersistence,
            EntityPersistence entityPersistence,
            ResidencyTick residencyTick,
            WorldContext worldContext,
            PlayerContext playerContext,
            CreationService creationService,
            SpawnService spawnService,
            CollisionService collisionService)
        {
            this.eventBus = eventBus;
            this.snapshotPersistence = snapshotPersistence;
            this.entityPersistence = entityPersistence;
            this.residencyTick = residencyTick;
            this.worldContext = worldContext;
            this.playerContext = playerContext;
            this.creationService = creationService;
            this.spawnService = spawnService;
            this.collisionService = collisionService;
        }

        #region Methods
        public async Task<PlayerInstance> CreateNewPlayer(
            string playerDefinitionId,
            string roomDefinitionId,
            string userId)
        {
            // Create new player instance and expand linking rooms
            var creation = creationService.CreatePlayer(
                playerDefinitionId,
                roomDefinitionId,
                userId);

            // Persist the snapshot first then reload later
            await snapshotPersistence.SaveWorldSnapshotAsync(
                new WorldSnapshot()
                {
                    Entities = creation.Entities,
                    Rooms = creation.Rooms,
                });

            // Return for loading later
            return creation.Player;
        }

        public async Task<(PlayerInstance, RoomSnapshot)> LoadExistedPlayer(
            string playerInstanceId,
            string userId)
        {
            // Load player persistence
            var player = await entityPersistence.LoadAsync<PlayerInstance>(playerInstanceId);
            if (player == null)
                throw new InternalException(
                    ResponseCode.PlayerCoordinator_PlayerInstanceNotFoundInPersistence,
                    $"Player instance on load with instance ID: {playerInstanceId} is not found");

            // Validate ownership
            if (userId != player.UserID)
                throw new Unauthorized(
                    ResponseCode.PlayerCoordinator_UnauthorizedPlayerInstance,
                    $"Player session is unauthorized");

            // Ensure room envirommnet is resident
            var snapshot = await residencyTick.EnsureRoomLoaded(player.RoomSpatialID);

            // Get collision result of current spawn
            var collision = collisionService.QueryPoint(
                shape: player.CollisionShape,
                roomSpatialId: player.RoomSpatialID,
                position: player.Position,
                layerZ: player.LayerZ);

            // Validate collision result
            if (collision.IsBlocked)
                throw new InternalException(
                    ResponseCode.PlayerCoordinator_NoValidSpawn,
                    $"No vaild spawn for room with spatail ID: {player.RoomSpatialID}");

            // Runtime mutation
            worldContext.AddEntity(player);

            // Membership graph
            playerContext.JoinRoom(player.RoomSpatialID, player.ID);

            // Publish spawn new player in room
            eventBus.Publish(new EntityLifecycleEvent(
                player,
                player.RoomSpatialID,
                EntityLifecycleType.Spawn));

            // Publish join room
            eventBus.Publish(new PlayerGroupedEvent(
                player.UserID,
                null,
                player.RoomSpatialID));

            // Mark active and return sync data for handler
            residencyTick.MarkRoomHot(player.RoomSpatialID);
            return (player, snapshot);
        }

        public async Task UnloadExistedPlayer(
            string playerInstanceId)
        {
            // Retrieve player instance need to be unloaded 
            var player = worldContext.GetEntity<PlayerInstance>(playerInstanceId);
            if (player == null)
                throw new InternalException(
                    ResponseCode.PlayerCoordinator_PlayerInstanceNotFoundInRuntime,
                    $"Player instance in runtime with instance ID: {playerInstanceId} is not found");

            // Persist player entity itself and remove player from runtime
            await entityPersistence.SaveAsync(player);
            worldContext.RemoveEntity(player.ID);

            // Membership graph
            playerContext.LeaveRoom(player.RoomSpatialID, player.ID);

            // Publish despawn player in room
            eventBus.Publish(new EntityLifecycleEvent(
                player,
                player.RoomSpatialID,
                EntityLifecycleType.Despawn));

            // Publish leave room
            eventBus.Publish(new PlayerGroupedEvent(
                player.UserID,
                player.RoomSpatialID,
                null));

            // Downgrade residency if empty
            if (playerContext.IsRoomEmpty(player.RoomSpatialID))
                residencyTick.MarkRoomExited(player.RoomSpatialID);
        }

        public async Task<RoomSnapshot> PlayerChangeRoom(
            string playerInstanceId,
            string targetRoomId)
        {
            // Retrieve player instance need to be unloaded 
            var player = worldContext.GetEntity<PlayerInstance>(playerInstanceId);
            if (player == null)
                throw new InternalException(
                    ResponseCode.PlayerCoordinator_PlayerInstanceNotFoundInRuntime,
                    $"Player instance in runtime with instance ID: {playerInstanceId} is not found");

            // Ensure target room environment is loaded into runtime
            var snapshot = await residencyTick.EnsureRoomLoaded(targetRoomId);
            var targetRoom = snapshot.Room;

            // Resolve new position for new room transistion
            var (position, layerZ) = spawnService.ResolveSpawnPosition(
                targetRoom.DefinitionID,
                player.DefinitionID,
                SpawnRuleType.Player);

            // Get collision result of new spawn
            var collision = collisionService.QueryPoint(
                shape: player.CollisionShape,
                roomSpatialId: targetRoomId,
                position: position,
                layerZ: layerZ);

            // Validate collision result
            if (collision.IsBlocked)
                throw new InternalException(
                    ResponseCode.PlayerCoordinator_NoValidSpawn,
                    $"No vaild spawn for room with spatail ID: {targetRoomId}");

            var fromRoomId = player.RoomSpatialID;

            // Runtime mutation
            worldContext.ChangeRoom(
                playerInstanceId,
                position,
                layerZ,
                targetRoomId);

            // Membership graph
            playerContext.LeaveRoom(
                fromRoomId,
                player.ID);

            playerContext.JoinRoom(
                targetRoomId,
                player.ID);

            // Residency transitions
            residencyTick.MarkRoomHot(targetRoomId);
            if (playerContext.IsRoomEmpty(fromRoomId))
                residencyTick.MarkRoomExited(fromRoomId);
            else
                residencyTick.TouchRoom(fromRoomId);

            // Publish changes
            eventBus.Publish(new EntityLifecycleEvent(
                player,
                fromRoomId,
                EntityLifecycleType.Despawn));

            eventBus.Publish(new EntityLifecycleEvent(
                player,
                targetRoomId,
                EntityLifecycleType.Spawn));

            eventBus.Publish(new PlayerGroupedEvent(
                player.UserID,
                fromRoomId,
                targetRoomId));

            // Return sync data for handler
            return snapshot;
        }
        #endregion
    }
}