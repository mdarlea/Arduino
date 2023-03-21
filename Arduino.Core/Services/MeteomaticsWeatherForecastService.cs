using Arduino.Core.Models;
using Arduino.Core.Models.Meteomatics;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Arduino.Core.Services
{
    public class MeteomaticsWeatherForecastService : IWeatherForecastService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public MeteomaticsWeatherForecastService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<string?> Login(string userName, string password)
        {
            using (var client = CreateHttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(
                        System.Text.Encoding.ASCII.GetBytes(
                           $"{userName}:{password}")));

                HttpResponseMessage result = await client.GetAsync("https://login.meteomatics.com/api/v1/token");
                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadAsStringAsync();

                    var token = JsonConvert.DeserializeObject<AccessTokenModel>(response);

                    return token?.AccessToken;
                }
            }

            return null;
        }

        public async Task<float?> GetOutdoorTemperature(OutdoorTemperatureRequest request)
        {
            var currentDateTime = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            var url = $"{currentDateTime}/t_2m:C/{request.Latitude},{request.Longitude}/json?access_token={request.AccessToken}";

            using (var client = CreateHttpClient())
            {
                HttpResponseMessage result = await client.GetAsync(url);

                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadAsStringAsync();

                    var data = JsonConvert.DeserializeObject<Response>(response);

                    if (data != null) 
                    {
                        return data.Data.First().Coordinates.First().Dates.First().Value;
                    }                    
                }
            }

            return null;
        }

        private HttpClient CreateHttpClient()
        {
            return httpClientFactory.CreateClient("WeatherForecast");
        }
    }
}
