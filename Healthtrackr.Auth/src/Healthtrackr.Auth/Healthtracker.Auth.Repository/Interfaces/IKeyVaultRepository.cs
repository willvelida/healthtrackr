using Azure.Security.KeyVault.Secrets;

namespace Healthtracker.Auth.Repository.Interfaces
{
    public interface IKeyVaultRepository
    {
        Task<KeyVaultSecret> GetSecret(string secretName);
        Task SaveSecret(string secretName, string secretValue);
    }
}
