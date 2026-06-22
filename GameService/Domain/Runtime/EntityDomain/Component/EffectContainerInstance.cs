using Domain.Abstraction;

namespace Domain.Runtime.EntityDomain.Component
{
    public class EffectContainerInstance : ComponentInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public List<EffectInstance> ActiveEffects { get; private set; }
        #endregion

        public EffectContainerInstance() : base(Guid.Empty)
        { 
            ActiveEffects = new List<EffectInstance>();
        }

        #region Methods
        #endregion
    }

    public class EffectInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public string DefinitionID { get; }
        public float? RemainingTime { get; private set; }
        public float? IntervalDuration { get; }
        public float IntervalAccumulator { get; private set; }

        public bool HasProcessedInitial { get; private set; }
        #endregion

        public EffectInstance(
            string definitionId,
            float? remainingTime,
            float? intervalDuration,
            float intervalAccumulator = 0f)
        {
            DefinitionID = definitionId;
            RemainingTime = remainingTime;
            IntervalDuration = intervalDuration;
            IntervalAccumulator = intervalAccumulator;
        }

        #region Methods
        public bool Tick(
            float deltaTime)
        {
            // Decrease Duration
            if (RemainingTime.HasValue)
            {
                RemainingTime -= deltaTime;
            }

            // Accumulate Interval
            if (IntervalDuration.HasValue)
            {
                IntervalAccumulator += deltaTime;
                if (IntervalAccumulator >= IntervalDuration.Value)
                {
                    IntervalAccumulator -= IntervalDuration.Value;
                    return true;
                }
            }

            return false;
        }

        public void ResetTimer(
            float duration)
        {
            RemainingTime = duration;
        }

        public void MarkProcessed() => HasProcessedInitial = true;

        public bool IsInstant() => !RemainingTime.HasValue;

        public bool IsExpired() => RemainingTime.HasValue && RemainingTime <= 0;
        #endregion
    }
}