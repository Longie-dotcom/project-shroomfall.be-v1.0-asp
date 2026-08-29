namespace Application.Interface.Realtime.Managers
{
    public interface ISessionManager
    {
        void Add(
            string userId, 
            string playerInstanceId);
        string? Remove(
            string userId);
        string? Get(
            string userId);
        string? GetUserIdByPlayerId(
            string playerInstanceId);
    }
}
