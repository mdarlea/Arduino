using Arduino.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Threading;

namespace Arduino
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer indoorTemperatureDispatcherTimer = new DispatcherTimer();
        private readonly DispatcherTimer outdoorTemperatureDispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            var currentApp = Application.Current as App;
            DataContext = currentApp!.ServiceProvider?.GetRequiredService<MainWindowViewModel>();

            indoorTemperatureDispatcherTimer.Tick += indoorTemperatureDispatcherTimer_Tick;
            indoorTemperatureDispatcherTimer.Interval = new TimeSpan(0, 5, 0);

            outdoorTemperatureDispatcherTimer.Tick += outdoorTemperatureDispatcherTimer_Tick;
            outdoorTemperatureDispatcherTimer.Interval = new TimeSpan(1, 0, 0);
        }

        public MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.IsActive= true;

            indoorTemperatureDispatcherTimer.Start();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.IsActive= false;

            indoorTemperatureDispatcherTimer.Stop();
        }

        private void indoorTemperatureDispatcherTimer_Tick(object? sender, EventArgs e)
        {
            ViewModel.IndoorTemperatureTimerTicked();            
        }

        private void outdoorTemperatureDispatcherTimer_Tick(object? sender, EventArgs e)
        {
            ViewModel.OutdoorTemperatureTimerTicked();
        }
    }
}
