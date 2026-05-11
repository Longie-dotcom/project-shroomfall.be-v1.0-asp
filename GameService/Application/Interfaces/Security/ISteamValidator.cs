namespace Application.Interfaces.Security
{
    public interface ISteamValidator
    {
        Task<string?> ValidateTicket(
            string ticket);
    }
}
