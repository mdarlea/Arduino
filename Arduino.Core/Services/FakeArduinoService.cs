using Arduino.Core.Models;

namespace Arduino.Core.Services
{
    public class FakeArduinoService : IArduinoService
    {
        public async Task<bool> Connect()
        {
            return await Task.FromResult(true);
        }

        public async Task<TemperatureResponse> GetTemperature(SendMessageToArduinoRequest request)
        {
            var response = await GetMessage(request);

            float temperature;
            float.TryParse(response.Response, out temperature);

            return new TemperatureResponse
            {
                Temperature = temperature,
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
            if (request.Key == "IMin") 
            {
                return Task.FromResult(new ArduinoResponse
                {
                    Response = "19",
                    ElapsedTime = 100,
                });
            }

            if (request.Key == "OMin")
            {
                return Task.FromResult(new ArduinoResponse
                {
                    Response = "10",
                    ElapsedTime = 100,
                });
            }

            if (request.Key == "IMax")
            {
                return Task.FromResult(new ArduinoResponse
                {
                    Response = "25",
                    ElapsedTime = 100,
                });
            }

            if (request.Key == "OMax")
            {
                return Task.FromResult(new ArduinoResponse
                {
                    Response = "30",
                    ElapsedTime = 100,
                });
            }

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
