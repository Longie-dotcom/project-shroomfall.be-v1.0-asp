using Application.Services.WorldService;
using Application.Systems.Abstraction;
using Domain.Common;

namespace Application.Systems.Queue
{
    public readonly struct MovementCommand : IEntityCommand
    {
        public string EntityInstanceID { get; }
        public CollisionBody Body { get; }
        public Vector2 DesiredPosition { get; }

        public MovementCommand(
            string entityInstanceId,
            CollisionBody body, 
            Vector2 desiredPosition)
        {
            EntityInstanceID = entityInstanceId;
            Body = body;
            DesiredPosition = desiredPosition;
        }
    }

    public readonly struct ItemActionCommand : IEntityCommand
    {
        public string EntityInstanceID { get; }
        public string ItemInstanceID { get; }
        public Vector2 TargetPosition { get; }

        public ItemActionCommand(
            string entityId,
            string itemId,
            Vector2 targetPosition)
        {
            EntityInstanceID = entityId;
            ItemInstanceID = itemId;
            TargetPosition = targetPosition;
        }
    }

    public struct EntityExpiredCommand : IEntityCommand
    {
        public string EntityInstanceID { get; }

        public EntityExpiredCommand(
            string entityInstanceId)
        {
            EntityInstanceID = entityInstanceId;
        }
    }
}