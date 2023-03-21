using Arduino.Core.Models;

namespace Arduino.Core.Services
{
    public interface IAccessTokenService
    {
        Task<string?> GetAccessToken(GetAccessTokenRequest request);
    }
}