using Azure.Security.KeyVault.Secrets;
using Healthtracker.Auth.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace Healthtracker.Auth.Repository
{
    public class KeyVaultRepository : IKeyVaultRepository
    {
        private readonly SecretClient _secretClient;
        private readonly ILogger<KeyVaultRepository> _logger;

        public KeyVaultRepository(SecretClient secretClient, ILogger<KeyVaultRepository> logger)
        {
            _secretClient = secretClient;
            _logger = logger;
        }

        public async Task<KeyVaultSecret> GetSecret(string secretName)
        {
            try
            {
                return await _secretClient.GetSecretAsync(secretName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetSecret)}: {ex.Message}");
                throw;
            }
        }

        public async Task SaveSecret(string secretName, string secretValue)
        {
            try
            {
                await _secretClient.SetSecretAsync(secretName, secretValue);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(SaveSecret)}: {ex.Message}");
                throw;
            }
        }
    }
}
