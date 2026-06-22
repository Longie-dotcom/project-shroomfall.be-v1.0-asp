using Domain.Abstraction;

namespace Domain.Snapshot.EntityDomain.Component
{
    public class ProjectileSnapshot : ComponentSnapshot
    {
        public float Velocity { get; set; }
        public string? OnImpactSpawnEntityDefinitionID { get; set; }
    }
}