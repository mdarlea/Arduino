namespace Arduino.Core.Models
{
    public class IndoorTemperatureResponse
    {
        public float? IndoorTemperature { get; set; }
        public long ElapsedTime { get; set; } = 0;
    }
}
