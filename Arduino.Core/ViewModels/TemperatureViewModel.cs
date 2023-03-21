using Arduino.Core.Messages;
using Arduino.Core.Models;
using Arduino.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Options;

namespace Arduino.Core.ViewModels
{
    public class TemperatureViewModel : ObservableRecipient
    {
        private static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private LocationResponse? location;

        private readonly IAccessTokenService accessTokenService;
        private readonly IWeatherForecastService weatherForecastService;
        private readonly IGeocodeService geocodeService;
        private readonly IArduinoService arduinoService;
        private readonly WeatherForecastSettings weatherForecastSettings;
        private readonly GeocodeSettings geocodeSettings;
        private readonly ArduinoSettings arduinoSettings;

        public TemperatureViewModel(
            IAccessTokenService accessTokenService, 
            IWeatherForecastService weatherForecastService, 
            IGeocodeService geocodeService,
            IArduinoService arduinoService,
            IOptions<WeatherForecastSettings> weatherForecastOptions,
            IOptions<GeocodeSettings> geocodeOptions,
            IOptions<ArduinoSettings> arduinoOptions)
        {
            this.accessTokenService = accessTokenService ?? throw new ArgumentNullException(nameof(accessTokenService));
            this.weatherForecastService = weatherForecastService ?? throw new ArgumentNullException(nameof(weatherForecastService));
            this.geocodeService = geocodeService ?? throw new ArgumentNullException(nameof(geocodeService));
            this.arduinoService = arduinoService ?? throw new ArgumentNullException(nameof(arduinoService));
            weatherForecastSettings = weatherForecastOptions.Value;
            geocodeSettings = geocodeOptions.Value;
            arduinoSettings= arduinoOptions.Value;
        }

        private float? outdoorTemperature;
        public float? OutdoorTemperature
        {
            get => outdoorTemperature;
            private set => SetProperty(ref outdoorTemperature, value, true);
        }

        private float? indoorTemperature;
        public float? IndoorTemperature
        {
            get => indoorTemperature;
            private set => SetProperty(ref indoorTemperature, value, true);
        }

        private bool displayTemperatures = false;
        public bool DisplayTemperatures
        {
            get => displayTemperatures;
            private set => SetProperty(ref displayTemperatures, value);
        }

        private int minOutdoorTemperature;
        public int MinOutdoorTemperature
        {
            get => minOutdoorTemperature;
            private set => SetProperty(ref minOutdoorTemperature, value);
        }

        private int maxOutdoorTemperature;
        public int MaxOutdoorTemperature
        {
            get => maxOutdoorTemperature;
            private set => SetProperty(ref maxOutdoorTemperature, value);
        }

        private int minIndoorTemperature;
        public int MinIndoorTemperature
        {
            get => minIndoorTemperature;
            private set => SetProperty(ref minIndoorTemperature, value);
        }

        private int maxIndoorTemperature;
        public int MaxIndoorTemperature
        {
            get => maxIndoorTemperature;
            private set => SetProperty(ref maxIndoorTemperature, value);
        }

        protected override async void OnActivated()
        {
            base.OnActivated();

            await GetIndoorMinTemperatureAsync();
            await GetOutdoorMinTemperatureAsync();

            await GetIndoorMaxTemperatureAsync();
            await GetOutdoorMaxTemperatureAsync();

            await GetOutdoorTemperatureAsync();
            await GetIndoorTemperatureAsync();

            DisplayTemperatures = true;

            Messenger.Register<TemperatureViewModel, AsyncOutdoorTemperatureRequestMessage>(this, (r, m) => m.Reply(r.GetOutdoorTemperatureAsync()));
            Messenger.Register<TemperatureViewModel, OutdoorTemperatureTimerTickedMessage>(this, async(r, m) => 
            {
                await GetOutdoorTemperatureAsync();
                Messenger.Send(new TemperaturesUpdatedMessage(true));
            });
            Messenger.Register<TemperatureViewModel, IndoorTemperatureTimerTickedMessage>(this, async (r, m) =>
            {
                await GetIndoorTemperatureAsync();
                Messenger.Send(new TemperaturesUpdatedMessage(true));
            });
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            Messenger.UnregisterAll(this);
        }

        private async Task GetIndoorTemperatureAsync() 
        {
            var response = await arduinoService.GetTemperature(new SendMessageToArduinoRequest 
            {
                Key = arduinoSettings.IndoorTemperatureKey
            });

            IndoorTemperature = response?.Temperature;
        }

        private async Task GetIndoorMinTemperatureAsync()
        {
            var response = await arduinoService.GetTemperature(new SendMessageToArduinoRequest
            {
                Key = arduinoSettings.MinIndoorTemperatureKey
            });

            if (response.Temperature.HasValue)
            {
                MinIndoorTemperature = (int)Math.Round(response.Temperature.Value);
            }
            else 
            {
                MinIndoorTemperature = 0;
            }
        }

        private async Task GetOutdoorMinTemperatureAsync()
        {
            var response = await arduinoService.GetTemperature(new SendMessageToArduinoRequest
            {
                Key = arduinoSettings.MinOutdoorTemperatureKey
            });

            if (response.Temperature.HasValue)
            {
                MinOutdoorTemperature = (int)Math.Round(response.Temperature.Value);
            }
            else
            {
                MinOutdoorTemperature = 0;
            }
        }

        private async Task GetIndoorMaxTemperatureAsync()
        {
            var response = await arduinoService.GetTemperature(new SendMessageToArduinoRequest
            {
                Key = arduinoSettings.MaxIndoorTemperatureKey
            });

            if (response.Temperature.HasValue)
            {
                MaxIndoorTemperature = (int)Math.Round(response.Temperature.Value);
            }
            else
            {
                MaxIndoorTemperature = 0;
            }
        }

        private async Task GetOutdoorMaxTemperatureAsync()
        {
            var response = await arduinoService.GetTemperature(new SendMessageToArduinoRequest
            {
                Key = arduinoSettings.MaxOutdoorTemperatureKey
            });

            if (response.Temperature.HasValue)
            {
                MaxOutdoorTemperature = (int)Math.Round(response.Temperature.Value);
            }
            else
            {
                MaxOutdoorTemperature = 0;
            }
        }


        private async Task<float?> GetOutdoorTemperatureAsync() 
        {
            if (location == null)
            {
                await semaphoreSlim.WaitAsync();
                try
                {
                    if (location == null)
                    {
                        location = await geocodeService.GetLocationForAddress(geocodeSettings.Address);
                    }
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            }

            if (location != null) 
            {
                var accessToken = await accessTokenService.GetAccessToken(new GetAccessTokenRequest
                {
                    TokenExpirationInMinutes = weatherForecastSettings.TokenExpirationInMinutes,
                    UserName = weatherForecastSettings.UserName!,
                    Password = weatherForecastSettings.Password!
                });

                if (!string.IsNullOrEmpty(accessToken))
                {
                    var temperature = await weatherForecastService.GetOutdoorTemperature(new OutdoorTemperatureRequest
                    {
                        AccessToken = accessToken,
                        Latitude = location.Latitude, 
                        Longitude = location.Longitude,
                    });

                    OutdoorTemperature = temperature;

                    return temperature;
                }
            }
            
            OutdoorTemperature = null;

            return null;
        }      

    }
}
