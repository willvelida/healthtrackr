using Healthtracker.Auth.Repository.Interfaces;
using Healthtrackr.Auth.Common.Models;
using Healthtrackr.Auth.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Healthtrackr.Auth.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IKeyVaultRepository _keyVaultRepository;
        private readonly HttpClient _httpClient;
        private readonly ILogger<RefreshTokenService> _logger;

        public RefreshTokenService(IKeyVaultRepository keyVaultRepository, HttpClient httpClient, ILogger<RefreshTokenService> logger)
        {
            _keyVaultRepository = keyVaultRepository;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<RefreshTokenResponse> RefreshTokens()
        {
            try
            {
                var fitbitRefreshTokenSecret = await _keyVaultRepository.GetSecret("RefreshToken");
                if (fitbitRefreshTokenSecret is null)
                    throw new NullReferenceException("Fitbit refresh token could not be retrieved. Verify that the secret exists in Key Vault.");

                var fitbitClientCredentials = await _keyVaultRepository.GetSecret("FitbitCredentials");
                if (fitbitClientCredentials is null)
                    throw new NullReferenceException("Fibit client credentials could not be retrieved. Verify that the secret exists in Key Vault.");

                _httpClient.DefaultRequestHeaders.Clear();
                UriBuilder uri = new UriBuilder("https://api.fitbit.com/oauth2/token");
                uri.Query = $"grant_type=refresh_token&refresh_token={fitbitRefreshTokenSecret.Value}";
                var request = new HttpRequestMessage(HttpMethod.Post, uri.Uri);
                request.Content = new StringContent("");
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", fitbitClientCredentials.Value);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                _logger.LogInformation("Fitbit API called successfully. Parsing response");

                var content = await response.Content.ReadAsStringAsync();
                var refreshTokenResponse = JsonConvert.DeserializeObject<RefreshTokenResponse>(content);

                return refreshTokenResponse;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Exception thrown in {nameof(RefreshTokenService)}: {ex.Message}");
                throw;
            }
        }

        public async Task SaveTokens(RefreshTokenResponse token)
        {
            try
            {
                _logger.LogInformation("Saving tokens to Key Vault");
                await _keyVaultRepository.SaveSecret("RefreshToken", token.RefreshToken);
                await _keyVaultRepository.SaveSecret("AccessToken", token.AccessToken);
                _logger.LogInformation("Tokens sucessfully saved to Key Vault");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(SaveTokens)}: {ex.Message}");
                throw;
            }
        }
    }
}
