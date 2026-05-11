namespace Application.Interfaces.Realtime
{
    public interface IConnectionManager
    {
        Task JoinAsync(string connectionId, string groupId);
        Task LeaveAsync(string connectionId, string groupId);
    }
}
