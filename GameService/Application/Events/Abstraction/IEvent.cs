namespace Application.Events.Abstraction
{
    public interface IEvent
    {
        DateTime OccurredAt { get; }
    }
}
