namespace Application.Interfaces.Realtime
{
    public interface IConnectionRegistry
    {
        void Add(
            string userId,
            string connectionId);
        string? Remove(
            string userId);
        string? Get(
            string userId);
    }
}
