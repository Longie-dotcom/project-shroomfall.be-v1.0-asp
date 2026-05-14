using Application.Events.Abstraction;

namespace Application.Interfaces.Realtime
{
    public interface IEventBus
    {
        void Publish(
            IEvent @event);
        List<IEvent> Drain();
    }
}
