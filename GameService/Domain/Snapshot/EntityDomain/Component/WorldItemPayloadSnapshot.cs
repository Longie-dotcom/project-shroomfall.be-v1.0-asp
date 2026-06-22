using Domain.Abstraction;

namespace Domain.Snapshot.EntityDomain.Component
{
    public class WorldItemPayloadSnapshot : ComponentSnapshot
    {
        public ItemSnapshot Payload { get; set; } = new ItemSnapshot();
    }
}