using Arduino.Core.Models;

namespace Arduino.Core.Services
{
    public interface IArduinoService: IDisposable
    {
        Task WriteMessage(SendMessageToArduinoRequest request);
        Task<ArduinoResponse> GetMessage(SendMessageToArduinoRequest request);
        Task<TemperatureResponse> GetTemperature(SendMessageToArduinoRequest request);
        Task<bool> Connect();
    }
}