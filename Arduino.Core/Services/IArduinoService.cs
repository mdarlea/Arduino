using Arduino.Core.Models;

namespace Arduino.Core.Services
{
    public interface IArduinoService: IDisposable
    {
        Task WriteMessage(SendMessageToArduinoRequest request);
        Task<ArduinoResponse> GetMessage(SendMessageToArduinoRequest request);
        Task<IndoorTemperatureResponse> GetIndoorTemperature(SendMessageToArduinoRequest request);
        Task<bool> Connect();
    }
}