namespace Application.Interfaces.Realtime.Events
{
    public interface IEventBus
    {
        void Publish(
            IEvent @event);
        List<IEvent> Drain();
    }
}
