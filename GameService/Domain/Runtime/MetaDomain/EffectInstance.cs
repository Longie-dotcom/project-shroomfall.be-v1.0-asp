using Contract.DTO.Definition.MetaDomain;
using Domain.Runtime.EntityDomain;

namespace Domain.Runtime.MetaDomain
{
    public class EffectContext
    {
        public required EntityInstance Target { get; init; }
        public EntityInstance? Source { get; init; } // null mean effect belong to target (self consume)
        public required EffectDefinitionDTO Effect { get; init; }
    }

    public class EffectInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public string DefinitionID { get; }
        public EffectContext Context { get; }
        public float? RemainingTime { get; private set; }
        public float? IntervalDuration { get; }
        public float IntervalAccumulator { get; private set; }
        #endregion

        public EffectInstance(
            string definitionId,
            EffectContext effectContext,
            float? remainingTime,
            float? intervalDuration,
            float intervalAccumulator = 0f)
        {
            DefinitionID = definitionId;
            Context = effectContext;
            RemainingTime = remainingTime;
            IntervalDuration = intervalDuration;
            IntervalAccumulator = intervalAccumulator;
        }

        #region Methods
        public bool TickInterval(
            float deltaTime)
        {
            if (!IntervalDuration.HasValue) return false;

            IntervalAccumulator += deltaTime;
            if (IntervalAccumulator >= IntervalDuration.Value)
            {
                IntervalAccumulator -= IntervalDuration.Value;
                return true;
            }
            return false;
        }

        public void TickDuration(
            float deltaTime)
        {
            if (RemainingTime.HasValue)
            {
                RemainingTime -= deltaTime;
            }
        }

        public void ResetTimer(
            float duration)
        {
            RemainingTime = duration;
        }

        public bool IsExpired() => RemainingTime.HasValue && RemainingTime <= 0;
        #endregion
    }
}