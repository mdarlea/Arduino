using Arduino.Core.Models;
using Arduino.Core.Models.Meteomatics;

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
            var response = await GetMessage(request);

            float indoorTemperature;
            float.TryParse(response.Response, out indoorTemperature);

            return new IndoorTemperatureResponse
            {
                IndoorTemperature = indoorTemperature,
                ElapsedTime = 100
            };
        }

        public async Task WriteMessage(SendMessageToArduinoRequest request)
        {
            await Task.Delay(100);
        }

        public void Dispose()
        {
            
        }

        public Task<ArduinoResponse> GetMessage(SendMessageToArduinoRequest request)
        {
            Random rand = new Random();
            double min = 5;
            double max = 40;
            double range = max - min;

            double sample = rand.NextDouble();
            double scaled = (sample * range) + min;
            float f = (float)scaled;

            return Task.FromResult(new ArduinoResponse 
            {
                Response = f.ToString(),
                ElapsedTime = 100,
            });
        }
    }
}
