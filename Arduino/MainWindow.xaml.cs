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
        private readonly DispatcherTimer dispatcherTimer = new DispatcherTimer();
        
        public MainWindow()
        {
            InitializeComponent();

            var currentApp = Application.Current as App;
            DataContext = currentApp!.ServiceProvider?.GetRequiredService<MainWindowViewModel>();

            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer.Start();

            ViewModel.TimerTicked();
        }

        public MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.IsActive= true;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.IsActive= false;
        }

        private void dispatcherTimer_Tick(object? sender, EventArgs e)
        {
            ViewModel.TimerTicked();
            
        }
    }
}
