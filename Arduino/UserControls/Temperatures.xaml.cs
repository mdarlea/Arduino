using Arduino.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace Arduino.UserControls
{
    /// <summary>
    /// Interaction logic for Temperatures.xaml
    /// </summary>
    public partial class Temperatures : UserControl
    {
        public Temperatures()
        {
            InitializeComponent();

            var currentApp = Application.Current as App;
            DataContext = currentApp!.ServiceProvider?.GetRequiredService<TemperatureViewModel>();
        }

        public TemperatureViewModel ViewModel => (TemperatureViewModel)DataContext;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.IsActive = true;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.IsActive = false;
        }
    }
}
