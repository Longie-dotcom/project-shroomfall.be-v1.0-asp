namespace Application.Interface.Realtime.Events
{
    public interface IEventDispatcher
    {
        Task Dispatch(
            IEvent @event);
    }
}
