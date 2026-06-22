using Domain.Abstraction;

namespace Domain.Snapshot.EntityDomain.Component
{
    public class EffectContainerSnapshot : ComponentSnapshot
    {
        public List<EffectSnapshot> ActiveEffects { get; set; } = new();
    }

    public class EffectSnapshot
    {
        public string DefinitionID { get; set; } = string.Empty;
        public float? RemainingTime { get; set; }
        public float IntervalAccumulator { get; set; }
    }
}