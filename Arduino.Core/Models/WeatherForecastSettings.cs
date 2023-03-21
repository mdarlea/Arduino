namespace Arduino.Core.Models
{
    public class WeatherForecastSettings
    {
        public string? BaseAddress { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public int TokenExpirationInMinutes { get; set; }
    }
}
