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
        }

        #region Methods
        public void Tick(
            float deltaTime)
        {
            if (RemainingTime == null) return;

            RemainingTime -= deltaTime;

            if (RemainingTime < 0)
                RemainingTime = 0;
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