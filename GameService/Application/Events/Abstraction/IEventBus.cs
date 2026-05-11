namespace Application.Events.Abstraction
{
    public interface IEventBus
    {
        void Publish(IEvent @event);
        List<IEvent> Drain();
    }
}
