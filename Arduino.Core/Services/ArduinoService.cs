using Arduino.Core.Models;
using System.Diagnostics;
using System.IO.Ports;

namespace Arduino.Core.Services
{
    public class ArduinoService : IDisposable, IArduinoService
    {
        private static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly SerialPort nanoPort;
        
        public ArduinoService(ArduinoConnection settings)
        {
            nanoPort = new SerialPort
            {
                BaudRate = settings.BaudRate,
                PortName = $"COM{settings.ComPortNumber}",
            };
        }

        public async Task<bool> Connect() 
        {
            if (!nanoPort.IsOpen) 
            {
                await semaphoreSlim.WaitAsync();
                try
                {
                    if (!nanoPort.IsOpen)
                    {
                        nanoPort.Open();
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
                finally 
                {
                    semaphoreSlim.Release();
                }
            }

            return true;
        }

        public async Task<IndoorTemperatureResponse> GetIndoorTemperature(SendMessageToArduinoRequest request)
        {
            await EnsureConnection();

            var watch = new Stopwatch();

            var temperature = await RequestValueFromArduino(watch, request.Key);

            if (!string.IsNullOrEmpty(temperature)) 
            {
                float indoorTemperature;
                float.TryParse(temperature, out indoorTemperature);

                return new IndoorTemperatureResponse
                {
                    IndoorTemperature = indoorTemperature,
                    ElapsedTime = watch.ElapsedMilliseconds
                };
            }

            return new IndoorTemperatureResponse();            
        }

        public async Task WriteMessage(SendMessageToArduinoRequest request)
        {
            await EnsureConnection();

            await semaphoreSlim.WaitAsync();

            try
            {
                nanoPort.DiscardInBuffer();

                nanoPort.Write($"<{request.Key},{request.IntValue},{request.FloatValue}>");
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private async Task EnsureConnection() 
        {
            if (nanoPort.IsOpen) 
            {
                return;
            }

            var connected = await Connect();

            if (!connected) 
            {
                throw new InvalidOperationException("Arduino is not connected to your computer");
            }
        }

        public void Dispose()
        {
            if (nanoPort.IsOpen)
            {
                nanoPort.Close();
            }

            nanoPort.Dispose();
        }

        public async Task<ArduinoResponse> GetMessage(SendMessageToArduinoRequest request)
        {
            await EnsureConnection();

            var watch = new Stopwatch();

            var value = await RequestValueFromArduino(watch, request.Key, request.IntValue, request.FloatValue);

            return new ArduinoResponse
            {
                Response = value,
                ElapsedTime = watch.ElapsedMilliseconds
            };
        }

        private async Task<string?> RequestValueFromArduino(Stopwatch watch, string key, int inputInt = 0, float inputFloat = 0)
        {
            string? value = null;

            await semaphoreSlim.WaitAsync();

            try
            {
                nanoPort.DiscardInBuffer();

                nanoPort.Write($"<{key},{inputInt},{inputFloat}>");

                watch.Start();
                value = await Task.Run(nanoPort.ReadLine);
                watch.Stop();
            }
            finally
            {
                semaphoreSlim.Release();
            }

            if (!string.IsNullOrEmpty(value))
            {
                value = value.Replace("\r", "").Replace("\n", "");
            }

            return value;
        }

    }
}
