namespace Arduino.Core.Models
{
    public class TemperatureResponse
    {
        public float? Temperature { get; set; }
        public long ElapsedTime { get; set; } = 0;
    }
}
