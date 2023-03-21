namespace Arduino.Core.Models.Meteomatics
{
    internal class Data
    {
        public string Parameter { get; set; } = string.Empty;
        public List<Coordinates> Coordinates { get; set; } = new();
    }
}
