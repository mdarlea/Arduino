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

        private bool loading;
        public bool Loading
        {
            get => loading;
            private set => SetProperty(ref loading, value);
        }

        public void TimerTicked() 
        {
            Loading = true;

            Messenger.Send(new TimerTickedMessage(DateTime.Now));
        }
        protected override void OnActivated()
        {
            base.OnActivated();

            Messenger.Register<MainWindowViewModel, TemperaturesUpdatedMessage>(this, (r, m) =>
            {
                if (m.Value) 
                {
                    Loading = false;
                }
            });

        }

        private async Task<float?> GetOutdoorTemperature() 
        {
            Loading = true;

            return await Messenger.Send<AsyncOutdoorTemperatureRequestMessage>();
        }
    }
}
