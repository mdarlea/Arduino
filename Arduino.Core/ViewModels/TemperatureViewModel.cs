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
            private set => SetProperty(ref indoorTemperature, value);
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            Messenger.Register<TemperatureViewModel, AsyncOutdoorTemperatureRequestMessage>(this, (r, m) => m.Reply(r.GetOutdoorTemperatureAsync()));
            Messenger.Register<TemperatureViewModel, TimerTickedMessage>(this, async(r, m) => 
            {
                await GetOutdoorTemperatureAsync();
                await GetIndoorTemperatureAsync(); 
            });
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            Messenger.UnregisterAll(this);
        }

        private async Task GetIndoorTemperatureAsync() 
        {
            var response = await arduinoService.GetIndoorTemperature(new SendMessageToArduinoRequest 
            {
                Key = arduinoSettings.IndoorTemperatureKey
            });

            IndoorTemperature = response?.IndoorTemperature;
        }

        private async Task<float?> GetOutdoorTemperatureAsync() 
        {
            var location = await geocodeService.GetLocationForAddress(geocodeSettings.Address);

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
