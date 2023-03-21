using Arduino.Core.Models;

namespace Arduino.Core.Services
{
    public class FakeArduinoService : IArduinoService
    {
        public async Task<bool> Connect()
        {
            return await Task.FromResult(true);
        }

        public async Task<IndoorTemperatureResponse> GetIndoorTemperature(SendMessageToArduinoRequest request)
        {
            return await Task.FromResult(new IndoorTemperatureResponse
            {
                IndoorTemperature = 24,
                ElapsedTime = 1000,
            });
        }

        public async Task WriteMessage(SendMessageToArduinoRequest request)
        {
            await Task.Delay(100);
        }

        public void Dispose()
        {
            
        }
    }
}
