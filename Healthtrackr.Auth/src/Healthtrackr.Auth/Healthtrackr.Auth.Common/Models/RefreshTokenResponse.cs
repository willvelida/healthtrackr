using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Healthtrackr.Auth.Common.Models
{
    [ExcludeFromCodeCoverage]
    public class RefreshTokenResponse
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }
        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }
        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty(PropertyName = "scope")]
        public string Scope { get; set; }
        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }
        [JsonProperty(PropertyName = "user_id")]
        public string UserType { get; set; }
    }
}
