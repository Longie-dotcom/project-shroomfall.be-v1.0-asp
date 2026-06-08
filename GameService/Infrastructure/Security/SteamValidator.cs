using Application.Interfaces.Security;
using System.Text.Json;

namespace Infrastructure.Security
{
    public class SteamValidator : ISteamValidator
    {
        #region Attributes
        private readonly HttpClient httpClient;
        private readonly string apiKey;
        private readonly string appId;
        #endregion

        #region Properties
        #endregion

        public SteamValidator(
            HttpClient httpClient,
            string apiKey,
            string appId)
        {
            this.httpClient = httpClient;
            this.apiKey = apiKey;
            this.appId = appId;
        }

        #region Methods
        public async Task<string?> ValidateTicket(string ticket)
        {
            var url =
                "https://api.steampowered.com/ISteamUserAuth/AuthenticateUserTicket/v1/" +
                $"?key={apiKey}&appid={appId}&ticket={ticket}";

            var response = await httpClient.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);

            var root = doc.RootElement.GetProperty("response");

            if (root.TryGetProperty("error", out var error))
            {
                var errorMsg = error.TryGetProperty("errordesc", out var desc)
                    ? desc.GetString()
                    : "Unknown Steam error";

                throw new Exception($"Steam validation failed: {errorMsg}");
            }

            if (!root.TryGetProperty("params", out var parameters))
                return null;

            if (!parameters.TryGetProperty("steamid", out var steamId))
                return null;

            return steamId.GetString();
        }
        #endregion
    }
}