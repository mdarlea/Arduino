namespace Arduino.Core.Models.Meteomatics
{
    internal class Response
    {
        public string Status { get; set; } = string.Empty;
        public List<Data> Data { get; set; } = new();
    }
}
