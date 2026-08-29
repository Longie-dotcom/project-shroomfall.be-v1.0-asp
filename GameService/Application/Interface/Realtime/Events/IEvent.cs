namespace Application.Interface.Realtime.Events
{
    public interface IEvent
    {
        DateTime OccurredAt { get; }
    }
}
