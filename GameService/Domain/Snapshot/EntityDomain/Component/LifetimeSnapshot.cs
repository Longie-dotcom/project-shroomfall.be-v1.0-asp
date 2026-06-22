using Domain.Abstraction;

namespace Domain.Snapshot.EntityDomain.Component
{
    public class LifetimeSnapshot : ComponentSnapshot
    {
        public float Duration { get; set; }
        public float ElapsedLifetime { get; set; }
    }
}