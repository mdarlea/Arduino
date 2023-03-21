using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Arduino.Core.Messages
{
    internal class IndoorTemperatureTimerTickedMessage : ValueChangedMessage<DateTime>
    {
        public IndoorTemperatureTimerTickedMessage(DateTime value) : base(value)
        {
        }
    }
}
