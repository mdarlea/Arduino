using Arduino.Core.Models;
using System.Runtime.Caching;

namespace Arduino.Core.Services
{
    public class AccessTokenService : IAccessTokenService
    {
        private const string cacheKey = "weather_forecast_access_token";

        private readonly IWeatherForecastService weatherForecastService;

        public AccessTokenService(IWeatherForecastService weatherForecastService)
        {
            this.weatherForecastService = weatherForecastService ?? throw new ArgumentNullException(nameof(weatherForecastService));
        }

        public async Task<string?> GetAccessToken(GetAccessTokenRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var cache = MemoryCache.Default;

            var token = cache[cacheKey] as string;

            if (string.IsNullOrEmpty(token))
            {
                var policy = new CacheItemPolicy();

                policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(request.TokenExpirationInMinutes);

                token = await weatherForecastService.Login(request.UserName, request.Password);

                if (!string.IsNullOrEmpty(token))
                {
                    cache.Set("cacheKey", token, policy);
                }
            }

            return token;
        }
    }
}
