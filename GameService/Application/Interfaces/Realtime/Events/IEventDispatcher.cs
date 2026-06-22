namespace Application.Interfaces.Realtime.Events
{
    public interface IEventDispatcher
    {
        Task Dispatch(
            IEvent @event);
    }
}
