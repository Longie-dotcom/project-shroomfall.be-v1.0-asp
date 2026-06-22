using Domain.Abstraction;

namespace Domain.Snapshot.EntityDomain.Component
{
    public class TriggeredEffectSnapshot : ComponentSnapshot
    {
        public List<string> EffectDefinitionIDs { get; set; } = new List<string>();
    }
}