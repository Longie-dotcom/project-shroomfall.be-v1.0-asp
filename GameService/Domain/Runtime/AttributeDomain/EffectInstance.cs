namespace Domain.Runtime.AttributeDomain
{
    public class EffectInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; }
        public string DefinitionID { get; }
        public float? RemainingTime { get; private set; }
        public string? SourceItemInstanceID { get; }
        public float IntervalAccumulator { get; private set; }
        #endregion

        public EffectInstance(
            string id,
            string definitionId,
            float? remainingTime,
            string? sourceItemInstanceId)
        {
            ID = id;
            DefinitionID = definitionId;
            RemainingTime = remainingTime;
            SourceItemInstanceID = sourceItemInstanceId;
            IntervalAccumulator = 0f;
        }

        #region Methods
        public void Tick(float deltaTime)
        {
            if (RemainingTime.HasValue)
            {
                RemainingTime -= deltaTime;
                if (RemainingTime < 0)
                    RemainingTime = 0;
            }

            IntervalAccumulator += deltaTime;
        }

        public bool TryConsumeInterval(float intervalDuration)
        {
            if (IntervalAccumulator >= intervalDuration)
            {
                IntervalAccumulator -= intervalDuration;
                return true;
            }
            return false;
        }

        public bool IsExpired()
        {
            return RemainingTime.HasValue && RemainingTime <= 0;
        }

        public bool IsPermanent()
        {
            return !RemainingTime.HasValue;
        }
        #endregion
    }
}