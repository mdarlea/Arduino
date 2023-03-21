using Arduino.Core.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Arduino.Core.ViewModels
{
    public class MainWindowViewModel : ObservableRecipient
    {
        public MainWindowViewModel() 
        {
            TemperatureCommand = new AsyncRelayCommand(GetOutdoorTemperature);
        }

        public IAsyncRelayCommand TemperatureCommand { get; }

        public void TimerTicked() 
        {
            Messenger.Send(new TimerTickedMessage(DateTime.Now));
        }

        private async Task<float?> GetOutdoorTemperature() 
        {
            return await Messenger.Send<AsyncOutdoorTemperatureRequestMessage>();
        }        
    }
}
