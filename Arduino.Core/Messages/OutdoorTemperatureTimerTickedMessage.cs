using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Arduino.Core.Messages
{
    internal class OutdoorTemperatureTimerTickedMessage : ValueChangedMessage<DateTime>
    {
        public OutdoorTemperatureTimerTickedMessage(DateTime value) : base(value)
        {
        }
    }
}
