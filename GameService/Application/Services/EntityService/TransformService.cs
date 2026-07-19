using Application.Services.WorldService;
using Application.Systems.Abstraction;
using Application.Systems.Queue;
using Contract.Enum.MetaDomain.Effect;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;

namespace Application.Services.EntityService
{
    public class TransformService : ITickService
    {
        #region Attributes
        private readonly WorldContext worldContext;
        #endregion

        #region Properties
        #endregion

        public TransformService(
            WorldContext worldContext)
        {
            this.worldContext = worldContext;
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
                {
                    commandBuffer.Commands.Enqueue(command.Value);
                }
            }
        }

        private MovementCommand? CreateMovementCommand(
                    float dt,
                    EntityInstance entity)
        {
            var transform = entity.GetComponent<TransformInstance>();
            if (transform == null)
                return null;

            var characteristic = entity.GetComponent<CharacteristicInstance>();
            if (characteristic == null)
                return null;

            var collision = entity.GetComponent<CollisionInstance>();
            if (collision == null)
                return null;

            if (!transform.WantsToMove)
                return null;

            float speed = characteristic.GetCore(AttributeType.MoveSpeed);
            var desired = transform.Position + transform.MovementVector * speed * dt;

            // Log the movement request to verify the calculation
            Console.WriteLine($"[Transform] Entity '{entity.ID}' | " +
                              $"Speed: {speed:F2} | " +
                              $"Vec: ({transform.MovementVector.X:F2}, {transform.MovementVector.Y:F2}) | " +
                              $"Current: ({transform.Position.X:F2}, {transform.Position.Y:F2}) -> " +
                              $"Desired: ({desired.X:F2}, {desired.Y:F2})");

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