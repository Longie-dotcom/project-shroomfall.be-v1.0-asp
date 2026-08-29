using Application.Interface.Realtime.Events;
using Application.Interface.Realtime.Events.Game;
using Application.Service.WorldService;
using Application.System.Abstraction;
using Application.System.Queue;
using Contract.Enum.MetaDomain.Effect;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;

namespace Application.Service.EntityService
{
    public class TransformService : ITickService
    {
        #region Attributes
        private readonly WorldContext worldContext;
        private readonly IEventBus eventBus;
        #endregion

        #region Properties
        #endregion

        public TransformService(
            WorldContext worldContext, 
            IEventBus eventBus)
        {
            this.worldContext = worldContext;
            this.eventBus = eventBus;
        }

        #region Methods
        public void Tick(
            float dt,
            CommandBuffer commandBuffer)
        {
            var entities = worldContext.GetEntities().ToList();
            foreach (var entity in entities)
            {
                var command = CreateMovementCommand(dt, entity);
                if (command != null)
                    commandBuffer.Commands.Enqueue(command.Value);
            }
        }

        private MovementCommand? CreateMovementCommand(
            float dt,
            EntityInstance entity)
        {
            var transform = entity.GetComponent<TransformInstance>();
            if (transform == null)
                return null;

            transform.TickActionLock(dt);

            var characteristic = entity.GetComponent<CharacteristicInstance>();
            if (characteristic == null)
                return null;

            var collision = entity.GetComponent<CollisionInstance>();
            if (collision == null)
                return null;

            // Handles synchronization for stationary actions (IDLE, SWING, SHOOT, ...).
            // Active movement (RUN) is handled separately by the movement/physics pipeline.
            if (!transform.WantsToMove)
            {
                if (transform.NeedsActionSync)
                {
                    eventBus.Publish(new EntityActedEvent(
                        entity.ID,
                        transform.RoomSpatialID,
                        transform.Position,
                        transform.FacingDirection,
                        transform.CurrentAction,
                        transform.ActiveItemDefinitionID
                    ));

                    transform.ClearActionSync(); 
                }

                return null;
            }

            float speed = characteristic.GetCore(AttributeType.MoveSpeed);
            var desired = transform.Position + transform.MovementVector * speed * dt;

            var body = new CollisionBody(
                entity.ID,
                transform.RoomSpatialID,
                transform.Position,
                collision.CollisionOffset,
                transform.LayerZ,
                collision.CollisionShape,
                collision.Layer,
                collision.Mask);

            return new MovementCommand(entity.ID, body, desired);
        }
        #endregion
    }
}