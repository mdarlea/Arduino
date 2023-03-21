namespace Arduino.Core.Models
{
    public class OutdoorTemperatureRequest
    {
        public string AccessToken { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
