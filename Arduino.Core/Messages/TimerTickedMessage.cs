using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Arduino.Core.Messages
{
    internal class TimerTickedMessage : ValueChangedMessage<DateTime>
    {
        public TimerTickedMessage(DateTime value) : base(value)
        {
        }
    }
}
