using Application.Context;
using Application.Features.Abstraction;
using Application.Features.Game.Commands;
using Application.Interfaces.Realtime.Managers;
using Application.Persistence;
using Application.Services.WorldService;
using AutoMapper;
using Contract.DTO.Connection;
using Contract.DTO.Domain.Runtime;
using Domain.Common;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.WorldDomain.Topology;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Application.Features.Game.Handlers
{
    public class TouchEntityHandler : IHandler<TouchEntityCommand, RoomSnapshotDTO>
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly ISessionManager sessionManager;
        private readonly WorldContext worldContext;
        private readonly ResidencyService residencyService;
        private readonly PlayerContext playerContext;
        private readonly EntitySpawnService entitySpawnService;
        private readonly IConnectionManager connectionManager; 
        private readonly TopologyService topologyService;
        private readonly SnapshotPersistence snapshotPersistence;
        private readonly RoomConnectionPersistence roomConnectionPersistence;
        #endregion

        public TouchEntityHandler(
            IMapper mapper,
            ISessionManager sessionManager,
            WorldContext worldContext,
            ResidencyService residencyService,
            PlayerContext playerContext,
            EntitySpawnService entitySpawnService,
            IConnectionManager connectionManager,
            TopologyService topologyService,
            SnapshotPersistence snapshotPersistence,
            RoomConnectionPersistence roomConnectionPersistence)
        {
            this.mapper = mapper;
            this.sessionManager = sessionManager;
            this.worldContext = worldContext;
            this.residencyService = residencyService;
            this.playerContext = playerContext;
            this.entitySpawnService = entitySpawnService;
            this.connectionManager = connectionManager;
            this.topologyService = topologyService;
            this.snapshotPersistence = snapshotPersistence;
            this.roomConnectionPersistence = roomConnectionPersistence;
        }

        #region Methods
        public async Task<RoomSnapshotDTO> Handle(
            TouchEntityCommand command)
        {
            // Validate session existence
            var playerInstanceId = sessionManager.Get(command.UserID);
            if (playerInstanceId == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.TouchEntitySessionNotFound,
                    $"Session was not found when changing room for user ID: {command.UserID}");

            // Validate player instance existence
            var player = worldContext.GetEntity(playerInstanceId);
            if (player == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.TouchEntityPlayerInstanceNotFound,
                    $"Player instance in runtime with instance ID: {playerInstanceId} is not found");

            // Validate transform existence
            var transform = player.GetComponent<TransformInstance>();
            if (transform == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.TouchEntityTransformMissing,
                    $"Player instance {player.ID} is missing TransformInstance component");

            // Return new room data for reconstruction
            var snapshot = await PlayerTouchEntity(player, transform, command.TouchedEntityInstanceID);

            // Build snapshot data transfer
            var snapshotDto = BuildDTO(snapshot);

            return snapshotDto;
        }

        public async Task<RoomSnapshot> PlayerTouchEntity(
            EntityInstance player,
            TransformInstance transform,
            string targetEntityInstanceId)
        {
            var (connectionForward, connectionReverse, roomSnapshot, isNew) =
                await topologyService.ResolveOrCreateConnection(targetEntityInstanceId);

            if (isNew && roomSnapshot != null)
            {
                await snapshotPersistence.SaveRoomSnapshotAsync(roomSnapshot);
                await roomConnectionPersistence.SaveAsync(connectionForward);
                await roomConnectionPersistence.SaveAsync(connectionReverse!);

                worldContext.AddConnection(connectionForward);
                worldContext.AddConnection(connectionReverse!);
            }

            return await PlayerChangeRoom(player, transform, connectionForward);
        }

        private async Task<RoomSnapshot> PlayerChangeRoom(
            EntityInstance player,
            TransformInstance transform,
            RoomConnectionInstance connection)
        {
            var toRoomId = connection.DestinationRoomSpatialID!;

            var fromRoomId = transform.RoomSpatialID;

            // Bring the room into RAM
            var snapshot = await residencyService.EnsureRoomLoaded(toRoomId);

            // PASS THE CONNECTION to the spawn resolver so we can target the portal anchor
            var (position, layerZ) = ResolveValidSpawn(connection, snapshot);

            // Execute spatial migration
            entitySpawnService.TransitionRoom(player, toRoomId, position, layerZ);

            // Update memory tracking states
            ApplyMembershipTransition(player, fromRoomId, toRoomId);
            ApplyResidencyTransition(fromRoomId, toRoomId);

            // Route socket pipes directly
            var ownership = player.GetComponent<OwnershipInstance>();
            if (ownership != null)
            {
                var activeConnections = connectionManager.Get(ownership.UserID);
                foreach (var connectionId in activeConnections)
                {
                    await connectionManager.Ungroup(connectionId, fromRoomId);
                    await connectionManager.Group(connectionId, toRoomId);
                }
            }

            return snapshot;
        }

        private (Vector2 Position, int LayerZ) ResolveValidSpawn(
            RoomConnectionInstance connection,
            RoomSnapshot snapshot)
        {
            // Find the actual physical landing portal instance inside the newly loaded room array
            var landingPortal = snapshot.Entities
                .FirstOrDefault(e => e.ID == connection.DestinationEntityInstanceID);

            if (landingPortal == null)
            {
                // Fallback safety boundary line if entity data is somehow corrupted
                return (Vector2.Zero, 0);
            }

            var portalTransform = landingPortal.GetComponent<TransformInstance>();
            if (portalTransform == null) return (Vector2.Zero, 0);

            // Apply a safety offset (e.g., dropping the player 1 tile down along the Y-axis)
            // Adjust this value based on your tile dimensions (e.g., -1.0f or -16.0f if pixel-perfect tracking)
            var spawnPosition = new Vector2(portalTransform.Position.X, portalTransform.Position.Y - 1.0f);

            return (spawnPosition, portalTransform.LayerZ);
        }

        private void ApplyMembershipTransition(
            EntityInstance player,
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
            residencyService.MarkRoomHot(toRoomId);

            if (playerContext.IsRoomEmpty(fromRoomId))
                residencyService.MarkRoomExited(fromRoomId);
            else
                residencyService.TouchRoom(fromRoomId);
        }

        private RoomSnapshotDTO BuildDTO(
            RoomSnapshot snapshot)
        {
            var snapshotDto = new RoomSnapshotDTO()
            {
                RoomData = mapper.Map<RoomRuntimeDTO>(snapshot.Room)
            };

            snapshotDto.RoomData.Entities = mapper.Map<List<EntityInstanceDTO>>(snapshot.Entities);
            
            return snapshotDto;
        }
        #endregion
    }
}