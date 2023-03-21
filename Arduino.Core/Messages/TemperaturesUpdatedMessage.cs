using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Arduino.Core.Messages
{
    internal class TemperaturesUpdatedMessage : ValueChangedMessage<bool>
    {
        public TemperaturesUpdatedMessage(bool value) : base(value)
        {
        }
    }
}
