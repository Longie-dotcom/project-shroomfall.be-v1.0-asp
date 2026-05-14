using Application.Events.Abstraction;

namespace Application.Interfaces.Realtime
{
    public interface IEventDispatcher
    {
        Task Dispatch(
            IEvent @event);
    }
}
