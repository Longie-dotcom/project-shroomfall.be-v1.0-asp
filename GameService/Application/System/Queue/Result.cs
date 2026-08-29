using Application.Service.MetaService;
using Application.System.Abstraction;
using Contract.Common;
using Domain.Runtime.EntityDomain;

namespace Application.System.Queue
{
    public readonly struct MovementResult : IEntityResult
    {
        #region Properties
        public string EntityInstanceID { get; }
        public Vector2 FinalPosition { get; }
        public int LayerZ { get; }
        public HashSet<EntityInstance> TriggeredEntities { get; }
        #endregion

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
        #region Properties
        public string EntityInstanceID { get; }
        public ItemUsageActionContext Context { get; }
        #endregion

        public ItemActionResult(
            string entityInstanceId,
            ItemUsageActionContext context)
        {
            EntityInstanceID = entityInstanceId;
            Context = context;
        }
    }

    public class EntityExpiredResult : IEntityResult
    {
        #region Properties
        public string EntityInstanceID { get; }
        #endregion

        public EntityExpiredResult(
            string entityInstanceID)
        {
            EntityInstanceID = entityInstanceID;
        }
    }

    public class VitalThresholdResult : IEntityResult
    {
        #region Properties
        public string EntityInstanceID { get; } = string.Empty;
        public DeathOutcome Outcome { get; }
        #endregion

        public VitalThresholdResult(
            string entityInstanceId,
            DeathOutcome outcome)
        {
            EntityInstanceID = entityInstanceId;
            Outcome = outcome;
        }
    }

    public class EntityDespawnResult : IEntityResult
    {
        #region Properties
        public string EntityInstanceID { get; } = string.Empty;
        public bool TriggerDeathLogic { get; }
        #endregion

        public EntityDespawnResult(
            string entityInstanceId,
            bool triggerDeathLogic)
        {
            EntityInstanceID = entityInstanceId;
            TriggerDeathLogic = triggerDeathLogic;
        }
    }
}