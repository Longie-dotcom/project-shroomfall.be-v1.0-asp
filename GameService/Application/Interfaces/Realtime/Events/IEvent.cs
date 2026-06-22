namespace Application.Interfaces.Realtime.Events
{
    public interface IEvent
    {
        DateTime OccurredAt { get; }
    }
}
