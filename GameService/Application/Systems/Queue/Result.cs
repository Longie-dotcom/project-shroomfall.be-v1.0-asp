using Application.Services.UsageService;
using Application.Systems.Abstraction;
using Domain.Common;
using Domain.Runtime.EntityDomain;

namespace Application.Systems.Queue
{
    public readonly struct MovementResult : IEntityResult
    {
        public string EntityInstanceID { get; }
        public Vector2 FinalPosition { get; }
        public int LayerZ { get; }
        public HashSet<EntityInstance> TriggeredEntities { get; }

        public MovementResult(
            string entityInstanceID,
            Vector2 finalPosition,
            int layerZ,
            HashSet<EntityInstance> triggeredEntities)
        {
            EntityInstanceID = entityInstanceID;
            FinalPosition = finalPosition;
            LayerZ = layerZ;
            TriggeredEntities = triggeredEntities;
        }
    }

    public readonly struct ItemActionResult : IEntityResult
    {
        public string EntityInstanceID { get; }
        public ItemUsageActionContext Context { get; }

        public ItemActionResult(
            string entityInstanceId,
            ItemUsageActionContext context)
        {
            EntityInstanceID = entityInstanceId;
            Context = context;
        }
    }

    public class DespawnResult : IEntityResult
    {
        public string EntityInstanceID { get; }

        public DespawnResult(
            string entityInstanceID)
        {
            EntityInstanceID = entityInstanceID;
        }
    }
}