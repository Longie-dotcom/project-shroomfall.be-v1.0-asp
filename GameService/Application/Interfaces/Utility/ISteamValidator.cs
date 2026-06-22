namespace Application.Interfaces.Utility
{
    public interface ISteamValidator
    {
        Task<string?> ValidateTicket(
            string ticket);
    }
}
