using Application.Events.Abstraction;

namespace Application.Interfaces.Realtime
{
    public interface IEventHandler
    {
        Task Handle(IEvent @event);
    }
}
