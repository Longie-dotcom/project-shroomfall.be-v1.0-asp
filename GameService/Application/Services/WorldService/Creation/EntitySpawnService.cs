using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Game;
using Application.Services.WorldService.Factory;
using Contract;
using Domain.Common;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.MetaDomain;
using ResponseCode;

namespace Application.Services.WorldService.Creation
{
    public class WorldEntityCreateContext
    {
        public string InstanceID { get; }
        public string DefinitionID { get; }
        public string RoomSpatialID { get; }
        public int LayerZ { get; }
        public Vector2 Position { get; }

        public WorldEntityCreateContext(
            string instanceId,
            string definitionId,
            string roomSpatialId,
            int layerZ,
            Vector2 position)
        {
            InstanceID = instanceId;
            DefinitionID = definitionId;
            RoomSpatialID = roomSpatialId;
            LayerZ = layerZ;
            Position = position;
        }
    }

    public class ProjectileEntityCreateContext : WorldEntityCreateContext
    {
        public Vector2 Direction { get; }
        public string SourceEntityID { get; }

        public ProjectileEntityCreateContext(
            string instanceId,
            string definitionId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 direction,
            string sourceEntityId) : base(
                instanceId, 
                definitionId,
                roomSpatialId, 
                layerZ,
                position)
        {
            Direction = direction;
            SourceEntityID = sourceEntityId;
        }
    }

    public class WorldItemCreateContext : WorldEntityCreateContext
    {
        public ItemInstance Payload { get; }

        public WorldItemCreateContext(
            string instanceId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            ItemInstance payload) : base(
                instanceId, 
                Constraint.DEFAULT_ENTITY_ITEM_DEFINITION_ID,
                roomSpatialId, 
                layerZ, 
                position)
        {
            Payload = payload;
        }
    }

    public class PlayerEntityCreateContext : WorldEntityCreateContext
    {
        public string UserID { get; }
        public string PersonalRoomID { get; }

        public PlayerEntityCreateContext(
            string instanceId,
            string definitionId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            string userId,
            string personalRoomID) : base(
                instanceId,
                definitionId,
                roomSpatialId,
                layerZ,
                position)
        {
            UserID = userId;
            PersonalRoomID = personalRoomID;
        }
    }

    public class EntitySpawnService
    {
        #region Attributes
        private readonly IEventBus eventBus;
        private readonly WorldContext worldContext;
        private readonly EntityInstanceFactory entityInstanceFactory;
        #endregion

        #region Properties
        #endregion

        public EntitySpawnService(
            IEventBus eventBus,
            WorldContext worldContext,
            EntityInstanceFactory entityInstanceFactory)
        {
            this.eventBus = eventBus;
            this.worldContext = worldContext;
            this.entityInstanceFactory = entityInstanceFactory;
        }

        #region Methods
        public void Spawn(
            WorldEntityCreateContext context)
        {
            var entity = entityInstanceFactory.Create(context);
            if (entity == null)
                throw new InternalException(
                    ApplicationCode.EntitySpawnServiceCode.SpawnEntityCreationFailed,
                    $"Failed to create entity instance from definition ID: {context.DefinitionID}");

            var transform = entity.GetComponent<TransformInstance>();
            if (transform == null)
                throw new InternalException(
                    ApplicationCode.EntitySpawnServiceCode.ActivateTransformMissing,
                    $"Cannot activate entity {entity.ID} without a Transform component.");

            // Inject into spatial partitioning and engine ticks
            worldContext.AddEntity(entity);

            // Inform netcode to render the instance for clients in range
            eventBus.Publish(new EntityLifecycleEvent(
                entity,
                transform.RoomSpatialID,
                EntityLifecycleType.Spawn));
        }

        public void Despawn(
            EntityInstance entity)
        {
            var transform = entity.GetComponent<TransformInstance>();
            if (transform == null) return;

            // Strip out from spatial partitioning and physics
            worldContext.RemoveEntity(entity.ID);

            // Inform netcode to tell clients to delete their local visual actor
            eventBus.Publish(new EntityLifecycleEvent(
                entity,
                transform.RoomSpatialID,
                EntityLifecycleType.Despawn));
        }

        public void TransitionRoom(
            EntityInstance entity,
            string targetRoomSpatialId,
            Vector2 targetPosition,
            int targetLayerZ)
        {
            var transform = entity.GetComponent<TransformInstance>();
            if (transform == null)
                throw new InternalException(
                    ApplicationCode.EntitySpawnServiceCode.TransitionTransformMissing,
                    $"Cannot transition entity {entity.ID} without a Transform component.");

            var oldRoomSpatialId = transform.RoomSpatialID;

            // FAREWELL PACKET (Old Room channel)
            // Alert current nearby clients using the pre-mutation location state properties
            eventBus.Publish(new EntityLifecycleEvent(
                entity,
                oldRoomSpatialId,
                EntityLifecycleType.Despawn));

            // ATOMIC DOMAIN ROOM TRANSITION
            // Delegates dictionary indexing mutations cleanly straight into World spatial boundaries
            worldContext.ChangeRoom(
                entity.ID,
                targetPosition,
                targetLayerZ,
                targetRoomSpatialId);

            // HELLO PACKET (New Room channel)
            // Alert newly targeted zone observers using the updated spatial coordinates
            eventBus.Publish(new EntityLifecycleEvent(
                entity,
                targetRoomSpatialId,
                EntityLifecycleType.Spawn));
        }
        #endregion
    }
}