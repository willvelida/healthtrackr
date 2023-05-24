using Healthtrackr.Auth.Common.Models;
using Healthtrackr.Auth.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Healthtrackr.Auth.Functions
{
    public class RefreshFitbitTokens
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ILogger<RefreshFitbitTokens> _logger;

        public RefreshFitbitTokens(IRefreshTokenService refreshTokenService, ILogger<RefreshFitbitTokens> logger)
        {
            _refreshTokenService = refreshTokenService;
            _logger = logger;
        }

        [Function(nameof(RefreshFitbitTokens))]
        public async Task Run([TimerTrigger("0 0 */6 * * *")] TimerInfo myTimer)
        {
            try
            {
                _logger.LogInformation($"Attempting to refresh Fitbit refresh and access tokens: {DateTime.Now}");

                RefreshTokenResponse refreshTokenResponse = await _refreshTokenService.RefreshTokens();

                _logger.LogInformation($"Refresh successful. Saving to Key Vault");
                await _refreshTokenService.SaveTokens(refreshTokenResponse);
                _logger.LogInformation("Tokens saved to Key Vault.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(RefreshFitbitTokens)}: {ex.Message}");
                throw;
            }
        }
    }
}
