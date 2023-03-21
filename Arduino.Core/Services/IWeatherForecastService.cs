using Arduino.Core.Models;

namespace Arduino.Core.Services
{
    public interface IWeatherForecastService
    {
        Task<float?> GetOutdoorTemperature(OutdoorTemperatureRequest request);
        Task<string?> Login(string userName, string password);
    }
}