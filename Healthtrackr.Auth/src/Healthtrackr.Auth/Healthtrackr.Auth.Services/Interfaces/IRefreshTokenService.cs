using Healthtrackr.Auth.Common.Models;

namespace Healthtrackr.Auth.Services.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<RefreshTokenResponse> RefreshTokens();
        Task SaveTokens(RefreshTokenResponse tokens);
    }
}
