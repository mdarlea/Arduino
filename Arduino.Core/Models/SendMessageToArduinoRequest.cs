namespace Arduino.Core.Models
{
    public class SendMessageToArduinoRequest
    {
        public string Key { get; set; } = string.Empty;
        public int IntValue { get; set; }
        public float FloatValue { get; set; }
    }
}
