﻿namespace Arduino.Core.Models
{
    public class ArduinoSettings
    {
        public ArduinoConnection? Connection {get;set; }
        public string IndoorTemperatureKey { get; set; } = string.Empty;
        public string OutdoorTemperatureKey { get; set; } = string.Empty;
        public string MinIndoorTemperatureKey { get; set; } = string.Empty;
        public string MinOutdoorTemperatureKey { get; set; } = string.Empty;
        public string MaxIndoorTemperatureKey { get; set; } = string.Empty;
        public string MaxOutdoorTemperatureKey { get; set; } = string.Empty;
    }

    public class ArduinoConnection 
    {
        public int ComPortNumber { get; set; }
        public int BaudRate { get; set; }
    }
}
