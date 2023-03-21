using Arduino.Core.Models;
using Arduino.Core.Services;
using Arduino.Core.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Arduino
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider? ServiceProvider { get; private set; }

        public IConfigurationRoot? Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddUserSecrets(Assembly.GetExecutingAssembly(), true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            var arduinoService = ServiceProvider?.GetService<IArduinoService>();
            arduinoService?.Dispose();
        }

        private void ConfigureServices(IServiceCollection services) 
        {           
            services.AddOptions<WeatherForecastSettings>().Configure((settings) =>
            {
                Configuration?.GetSection("WeatherForecast").Bind(settings);
            });
            services.AddOptions<GeocodeSettings>().Configure((settings) => 
            {
                Configuration?.GetSection("Geocode").Bind(settings);
            });

            services.AddHttpClient("WeatherForecast", httpClient =>
            {
                var settings = Configuration?.GetSection("WeatherForecast").Get<WeatherForecastSettings>();

                if (!string.IsNullOrEmpty(settings?.BaseAddress)) 
                {
                    httpClient.BaseAddress = new Uri(settings.BaseAddress);
                }                
            });

            //TODO: Enable when connecting an arduino to the PC
            //services.AddSingleton<IArduinoService, ArduinoService>(x =>
            //{
            //    var settings = Configuration?.GetSection("Arduino:Connection").Get<ArduinoConnection>();

            //    return new ArduinoService(settings!);
            //});
            services.AddSingleton<IArduinoService, FakeArduinoService>();

            services.AddTransient<IGeocodeService, MapQuestGeocodeService>(x =>
            {
                var settings = Configuration?.GetSection("Geocode").Get<GeocodeSettings>();

                return new MapQuestGeocodeService(settings!.MapQuestKey);
            });
            services.AddTransient<IAccessTokenService, AccessTokenService>();
            services.AddTransient<IWeatherForecastService, MeteomaticsWeatherForecastService>();

            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<TemperatureViewModel>();
        }
    }
}
