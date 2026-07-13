namespace Application.Interfaces.Realtime.Managers
{
    public interface IConnectionManager
    {
        void Add(
            string userId,
            string connectionId);
        void Remove(
            string userId,
            string connectionId);
        string? Get(
            string userId);
        Task Group(
            string connectionId,
            string groupId);
        Task Ungroup(
            string connectionId,
            string groupId);
    }
}
