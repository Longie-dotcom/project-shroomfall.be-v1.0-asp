namespace Domain.Runtime.MetaDomain
{
    public class EffectInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public string DefinitionID { get; }
        public float? RemainingTime { get; private set; }
        public float? IntervalDuration { get; }
        public float IntervalAccumulator { get; private set; }
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