using Application.Interfaces.Utility;
using Domain.DomainException;
using ResponseCode;
using System.Text.Json;

namespace Infrastructure.Utility
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
        public async Task<string?> ValidateTicket(
            string ticket)
        {
            var url = "https://api.steampowered.com/ISteamUserAuth/AuthenticateUserTicket/v1/" + $"?key={apiKey}&appid={appId}&ticket={ticket}";

            HttpResponseMessage response;
            try
            {
                response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    throw new InternalException(
                        InfrastructureCode.SteamValidatorCode.ConnectionFailed,
                        $"Steam API returned non-success status code: {response.StatusCode}");
            }
            catch (Exception ex) when (ex is not InternalException)
            {
                throw new InternalException(
                    InfrastructureCode.SteamValidatorCode.ConnectionError,
                    $"Failed to connect to Steam API: {ex.Message}");
            }

            var json = await response.Content.ReadAsStringAsync();

            Console.WriteLine("========== STEAM VALIDATOR ==========");
            Console.WriteLine($"AppId : {appId}");
            Console.WriteLine($"Ticket Length : {ticket.Length}");
            Console.WriteLine($"Ticket Prefix : {ticket[..Math.Min(ticket.Length, 32)]}");
            Console.WriteLine("Steam Response:");
            Console.WriteLine(json);
            Console.WriteLine("=====================================");
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement.GetProperty("response");

                if (root.TryGetProperty("error", out var error))
                {
                    var errorMsg = error.TryGetProperty("errordesc", out var desc)
                        ? desc.GetString()
                        : "Unknown Steam error";

                    throw new InternalException(
                        InfrastructureCode.SteamValidatorCode.SteamRejected,
                        $"Steam validation failed: {errorMsg}");
                }

                var parameters = root.GetProperty("params");
                return parameters.GetProperty("steamid").GetString();
            }
            catch (Exception ex) when (ex is not InternalException)
            {
                throw new InternalException(
                    InfrastructureCode.SteamValidatorCode.InvalidResponse,
                    $"Failed to process Steam response: {ex.Message}");
            }
        }
        #endregion
    }
}