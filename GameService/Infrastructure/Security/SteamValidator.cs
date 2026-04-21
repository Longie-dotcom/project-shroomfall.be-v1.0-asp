using System.Text.Json;

namespace Infrastructure.Security
{
    public class SteamValidator
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
        public async Task<string> ValidateTicket(string ticket)
        {
            var url = $"https://api.steampowered.com/ISteamUserAuth/AuthenticateUserTicket/v1/" +
                      $"?key={apiKey}&appid={appId}&ticket={ticket}";

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            // parse steamid (use System.Text.Json)
            using var doc = JsonDocument.Parse(json);

            var steamId = doc.RootElement
                .GetProperty("response")
                .GetProperty("params")
                .GetProperty("steamid")
                .GetString();

            if (string.IsNullOrEmpty(steamId))
                throw new Exception("Invalid Steam ticket");

            return steamId;
        }
        #endregion
    }
}