using Application.Services.UsageService;
using Application.Services.WorldService;
using Application.Systems.Abstraction;
using Contract.Enum.MetaDomain.Item;
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
        public ItemUsageActionContext Context { get; }

        public ItemActionCommand(
            string entityInstanceId,
            ItemUsageActionContext context)
        {
            EntityInstanceID= entityInstanceId;
            Context = context;
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