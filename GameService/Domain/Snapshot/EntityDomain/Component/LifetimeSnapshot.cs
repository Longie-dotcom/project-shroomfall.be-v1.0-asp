using Domain.Abstraction;

namespace Domain.Snapshot.EntityDomain.Component
{
    public class LifetimeSnapshot : ComponentSnapshot
    {
        public float ElapsedLifetime { get; set; }
    }
}