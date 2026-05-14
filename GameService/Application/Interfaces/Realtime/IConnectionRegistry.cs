namespace Application.Interfaces.Realtime
{
    public interface IConnectionRegistry
    {
        void Add(
            string userId,
            string connectionId);
        void Remove(
            string userId,
            string connectionId);
        IReadOnlyCollection<string> Get(
            string userId);
        bool HasConnections(
            string userId);
    }
}
